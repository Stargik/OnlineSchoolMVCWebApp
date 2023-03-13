using System.ComponentModel.DataAnnotations;

namespace OnlineSchoolMVCWebApp.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        public string Email { get; set; }
        [Display(Name = "Старий пароль")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        public string OldPassword { get; set; }
        [Display(Name = "Новий пароль")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        public string NewPassword { get; set; }


    }
}
