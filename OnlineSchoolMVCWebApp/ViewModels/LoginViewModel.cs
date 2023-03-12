using System.ComponentModel.DataAnnotations;

namespace OnlineSchoolMVCWebApp.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        public string Email { get; set; }

        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        public string Password { get; set; }

        [Display(Name = "Запам'ятати мене")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

    }
}
