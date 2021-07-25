using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Employee_Management.Models
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        { 
        new Claim("CreateRole","CreateRole"),
        new Claim("EditRole","EditRole"),
        new Claim("DeleteRole","DeleteRole"),


        };
    }
}

