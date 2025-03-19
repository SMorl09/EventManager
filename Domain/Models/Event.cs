using Domain.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string StartDate {  get; set; }
        public EventCategory Category { get; set; }
        public int MaxNumberOfUsers { get; set; }
        public ICollection<User>? Users { get; set; }
        public string? ImageUrl {  get; set; }
        [ForeignKey("Address")]
        public int? AddressId { get; set; }
        public Address? Address { get; set; }
    }
}
