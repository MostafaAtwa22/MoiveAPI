using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Services.DTOs.Roles
{
    public class CreateRoleDTO
    {
        [Required(ErrorMessage = "The Role name is required")]
        public string RoleName { get; set; } = string.Empty;
    }
}
