﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Employee_Management.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string city { get; set; }
    }
}
