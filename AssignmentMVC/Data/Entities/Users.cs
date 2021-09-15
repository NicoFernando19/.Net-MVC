using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentMVC.Data.Entities
{
    public class Users : IdentityUser
    {
        public string Name { get; set; }
    }
}
