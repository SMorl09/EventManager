using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Response
{
    public class EventWithAddressResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string StartDate { get; set; }
        public string Category { get; set; }
        public int MaxNumberOfUsers { get; set; }
        public string? ImageUrl { get; set; }
        public AddressResponse? Address { get; set; }
    }
}
