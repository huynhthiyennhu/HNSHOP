using System.ComponentModel.DataAnnotations;

namespace HNSHOP.Dtos.Request
{
    public class ForgotPasswordReqDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
