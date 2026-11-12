using System.ComponentModel.DataAnnotations;
namespace AlmostNewOutpost.Api.Models.Requests
{
    public class CreateUserRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [RegularExpression("^[A-Za-z0-9_]+$", ErrorMessage = "Username can only use letters, digits and underscores.")]

        public required string Username { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be 8 to 128 characters.")]

        public required string Password { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Choose a role.")]

        public int? RoleId { get; set; }



    }
}
