using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EntityLayer.Entity
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        /*
         * Navigation properties
         */
        [InverseProperty("Author")]
        public IList<BookAuthor> PublishedBooks { get; set; }
    }
}
