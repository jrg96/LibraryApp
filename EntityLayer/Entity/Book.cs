using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EntityLayer.Entity
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ISBN { get; set; }

        public int Stock { get; set; }


        /*
         * Navigation properties
         */
        [InverseProperty("Book")]
        public IList<BookAuthor> BookAuthors { get; set; }
    }
}
