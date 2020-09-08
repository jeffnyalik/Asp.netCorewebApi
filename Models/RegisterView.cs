using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class RegisterView
    {   
        [Required]
        [EmailAddress]
        public string Email  { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name="Username is required")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}