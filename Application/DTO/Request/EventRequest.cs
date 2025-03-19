using Domain.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Request
{
    public class EventRequest
    {
        [Required(ErrorMessage = "Event name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "The title must contain 3 to 100 characters")]
        public string Title { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "StartDate is required")]
        public string StartDate { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Max number of users must be greater than 0")]
        public int MaxNumberOfUsers { get; set; }
        public AddressRequest? Address { get; set; }
    }
}
