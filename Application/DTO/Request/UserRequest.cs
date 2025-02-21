using Domain.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Request
{
    public class UserRequest
    {
        [Required(ErrorMessage ="Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The name must contain 2 to 100 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
        [Required(ErrorMessage = "Surename is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The surename must contain 3 to 100 characters")]
        public string Surename { get; set; }
        [Required(ErrorMessage = "BirthDate is required")]
        public string BirthDate { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
    }
}
