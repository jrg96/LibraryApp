using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EntityLayer.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Utility;

namespace LibraryApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        /*
         * ------------------------------------------------------
         * CUSTOM IDENTITY CONFIGURATION
         * ------------------------------------------------------
         * 
         * STEP 1: Change the SignInManager and UserManager to ApplicationUser
         */
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        /*
         * ------------------------------------------------------
         * CUSTOM IDENTITY CONFIGURATION
         * ------------------------------------------------------
         * 
         * STEP 2: Add Dependency Injection for roles
         */
        private readonly RoleManager<IdentityRole> _roleManager;


        /*
         * ------------------------------------------------------
         * CUSTOM IDENTITY CONFIGURATION
         * ------------------------------------------------------
         * 
         * STEP 3: Change the Dependency injection to ApplicationUser
         *         and add Dependency Injection for RoleManager
         */
        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }


            /*
             * ------------------------------------------------------
             * CUSTOM IDENTITY CONFIGURATION
             * ------------------------------------------------------
             * 
             * STEP 4: Add the new fields added for the custom Identity
             */
            [Required]
            public string Name { get; set; }

            [Required]
            public string LastName { get; set; }

            public string Role { get; set; }

            /*
             * ------------------------------------------------------
             * CUSTOM IDENTITY CONFIGURATION
             * ------------------------------------------------------
             * 
             * STEP 5: Add the dropdown for role list to this ViewModel
             */
            public IEnumerable<SelectListItem> RoleList { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            /*
             * ------------------------------------------------------
             * CUSTOM IDENTITY CONFIGURATION
             * ------------------------------------------------------
             * 
             * STEP 6: Fill the dropdown (this dropdown will appear just for admins)
             */
            Input = new InputModel()
            {
                RoleList = (_roleManager.Roles)
                    .Select(i => i.Name)
                    .Select(i => new SelectListItem
                    {
                        Text = i,
                        Value = i
                    })
            };

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                /*
                 * ------------------------------------------------------
                 * CUSTOM IDENTITY CONFIGURATION
                 * ------------------------------------------------------
                 * 
                 * STEP 7: Create and fill custom application user
                 */
                var user = new ApplicationUser 
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Name = Input.Name,
                    LastName = Input.LastName,
                    Role = Input.Role
                };


                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    /*
                     * ------------------------------------------------------
                     * CUSTOM IDENTITY CONFIGURATION
                     * ------------------------------------------------------
                     * 
                     * STEP 8: We check if roles are already in database, if not, lets create them
                     */
                    if (!await _roleManager.RoleExistsAsync(CustomRoles.ROLE_ADMIN))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(CustomRoles.ROLE_ADMIN));
                    }

                    if (!await _roleManager.RoleExistsAsync(CustomRoles.ROLE_LIBRARIAN))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(CustomRoles.ROLE_LIBRARIAN));
                    }

                    if (!await _roleManager.RoleExistsAsync(CustomRoles.ROLE_NORMAL_USER))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(CustomRoles.ROLE_NORMAL_USER));
                    }


                    /*
                     * ------------------------------------------------------
                     * CUSTOM IDENTITY CONFIGURATION
                     * ------------------------------------------------------
                     * 
                     * STEP 9: Associate the role to the user
                     */

                    // If role is null, means the dropdown was never displayed (because its not an admin who is adding
                    // this user, just a normal register)
                    if (user.Role == null)
                    {
                        user.Role = CustomRoles.ROLE_NORMAL_USER;
                    }


                    // WARNING: THIS LINE ADDS AN ADMIN USER, BE CAREFUL
                    // await _userManager.AddToRoleAsync(user, CustomRoles.ROLE_ADMIN);

                    await _userManager.AddToRoleAsync(user, user.Role);

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
