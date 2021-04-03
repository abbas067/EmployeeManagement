using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Employee_Management.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required,MaxLength(5,ErrorMessage="Name cannot exceed 50 characters")]
        public string Name { get; set; }
        [Display(Name="Office Mail")]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$")]
        public string Email { get; set; }
        [Required]
        public Dept? Department { get; set; }
        public string Photopath { get; set; }
    }
}


