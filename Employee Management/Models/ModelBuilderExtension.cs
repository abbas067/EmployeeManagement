using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management.Models
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
               new Employee
               {
                   Id = 1,
                   Name = "Mark",
                   Department = Dept.It,
                   Email = "Mark@gmail.com"
               },
              new Employee
              {
                  Id = 2,
                  Name = "Mary",
                  Department = Dept.It,
                  Email = "Mary@gmail.com"
              }
               );
        }
    }
}
