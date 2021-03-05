using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Employee_Management.Models
{
    public class EmployeeCreateViewModel
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Display(Name = "Office Mail")]
        [Required]
        public string Email { get; set; }
        [Required]
        public Dept? Department { get; set; }
        public IFormFile Photo { get; set; }
    }
}
