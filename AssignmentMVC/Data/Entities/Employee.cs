using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentMVC.Data.Entities
{
    public class Employee : DateAt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        public string Address { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public string ImageName { get; set; }
        [Display(Name = "Date of Birth")]
        [Column(TypeName = "Date")]
        public DateTime DoB { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}
