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
        [Required,MaxLength(50)]
        public string Name { get; set; }
        [Display(Name="Office Mail")]
        [Required]
        public string Email { get; set; }
        [Required]
        public Dept? Department { get; set; }
        public string Photopath { get; set; }
    }
}


