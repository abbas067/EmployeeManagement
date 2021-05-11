using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Employee_Management.Models;
using Employee_Management.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management.Controllers
{
   [Authorize]
    public class HomeController : Controller
    {
        private IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment hostingEnvironment;

        public HomeController(IEmployeeRepository employeeRepository,IWebHostEnvironment hostingEnvironment)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
        }
        [AllowAnonymous]
        public ViewResult Index()
        {
            var model =_employeeRepository.GetAllEmployee();
            return View(model);
        }
       [HttpGet]
        public ViewResult Create()
        {

            return View();
        }
        [HttpGet] //edit action to handle get request
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.Photopath
            };
            return View(employeeEditViewModel);
        }
        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if (model.Photo != null)
                {
                  if  (model.ExistingPhotoPath!=null)
                    {
                       string filepath= Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);//find file path
                        System.IO.File.Delete(filepath);//delete file
                    }

                    employee.Photopath = ProceessUploadedFile(model);
                }
               
                _employeeRepository.Update(employee);
                return RedirectToAction("index");
            }
            return View();
        }

        private string ProceessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniquefilename = null;
            if (model.Photo != null)
            {
                string UploadsFolder = Path.Combine(hostingEnvironment.WebRootPath + "\\images");
                uniquefilename = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filepath = Path.Combine(UploadsFolder, uniquefilename);
                using(var filestream= new FileStream(filepath, FileMode.Create))
                    {
                    model.Photo.CopyTo(filestream);

                }
            }

            return uniquefilename;
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniquefilename = ProceessUploadedFile(model);
                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                Email = model.Email,
                Department = model.Department,
                Photopath=uniquefilename
            };
                _employeeRepository.Add(newEmployee);
 return RedirectToAction("details", new { id = newEmployee.Id });
            }
            return View();
        }
        [AllowAnonymous]
        public ViewResult Details(int? id)
        {
            Employee employee = _employeeRepository.GetEmployee(id.Value);
            if (employee==null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound",id.Value);
            }
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
                PageTitle = "Employee Details"

            };
            return View(homeDetailsViewModel);
        }
      
    }
}
