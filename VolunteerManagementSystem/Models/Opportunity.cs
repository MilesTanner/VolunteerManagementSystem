using System;
using System.ComponentModel.DataAnnotations;

namespace VolunteerManagementSystem.Models
{
    public class Opportunity
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Title { get; set; } = "";

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(120)]
        public string? Center { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
