using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EntityLayer.Entity
{
    public class BookAuthor
    {
        [Key]
        public int BookAuthorId { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }

        [ForeignKey("Author")]
        public int AuthorId { get; set; }

        /*
         * Navigation properties
         */
        public Book Book { get; set; }

        public Author Author { get; set; }
    }
}
