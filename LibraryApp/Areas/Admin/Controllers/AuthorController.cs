using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace LibraryApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = CustomRoles.ROLE_ADMIN)]
    public class AuthorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}