using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Request
{
    public class AddressRequest
    {
        public string? Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}
