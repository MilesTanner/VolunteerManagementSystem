using System.ComponentModel.DataAnnotations;

namespace VolunteerManagementSystem.Models
{
    public enum ApprovalStatus { Approved, PendingApproval, Disapproved, Inactive }

    public class Volunteer
    {
        public int Id { get; set; }

        [Required, StringLength(60)]
        public string FirstName { get; set; } = "";

        [Required, StringLength(60)]
        public string LastName { get; set; } = "";

        [EmailAddress, StringLength(120)]
        public string Email { get; set; } = "";

        [Required]
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.PendingApproval;
    }
}
