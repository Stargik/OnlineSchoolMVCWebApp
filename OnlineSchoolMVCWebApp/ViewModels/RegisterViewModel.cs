using System.ComponentModel.DataAnnotations;

namespace OnlineSchoolMVCWebApp.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Ім'я")]
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        public string FirstName { get; set; }

        [Display(Name = "Прізвище")]
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        public string Email { get; set; }

        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        [Display(Name = "Підтвердження паролю")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
