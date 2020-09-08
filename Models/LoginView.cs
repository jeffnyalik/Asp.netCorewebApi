using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class LoginView
    {   
        [Required]
        [StringLength(50)]
        [Display(Name ="username is required")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}