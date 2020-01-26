using LibraryData.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryData
{
    public class Patron
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        [Required]
        [StringLength(500)]
        public string Address { get; set; }
        [Required]
        
        public DateTime DateOfBirth { get; set; }
        [Required]
        [StringLength(20)]
        public string TelephoneNumber { get; set; }
      
        public LibraryCard LibraryCard { get; set; }
        public LibraryBranch HomeLibraryBranch { get; set; }

    }
}