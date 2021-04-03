using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Employee_Management.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController>logger)
        {
            this.logger = logger;
        }
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        { var statuscoderesult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch(statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage ="Sorry, the resource you requested could not be found";
                    logger.LogWarning($"404 error occured. path={statuscoderesult.OriginalPath}"+
                        $"and Query string ={statuscoderesult.OriginalQueryString}");
                    break;
            }
            return View("NotFound");
        }
        public IActionResult Error()
        {
            var exceptiondetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            logger.LogError($"the path {exceptiondetails.Path} threw an exception+" +
                $"{exceptiondetails.Error}");
            return View();
        }
    }
}
