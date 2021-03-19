using System.ComponentModel.DataAnnotations;

namespace IdentityAPI.Models.ViewModels.AccountViewModels
{
    public record ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; }
    }
}
