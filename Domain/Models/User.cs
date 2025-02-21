using Domain.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public RoleCategory Role { get; set; }
        public string Surename { get; set; }
        public string BirthDate { get; set; }
        public string CreatedDate { get; set; }
        public string Email { get; set; }
        public ICollection<Event>? Events { get; set; }

    }
}
