using Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Response
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surename { get; set; }
        public string BirthDate { get; set; }
        public string CreatedDate { get; set; }
        public string Email { get; set; }
    }
}
