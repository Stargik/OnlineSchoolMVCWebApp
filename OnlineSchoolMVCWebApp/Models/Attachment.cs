using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineSchoolMVCWebApp.Models;

public partial class Attachment
{
    [Display(Name = "Додатковий матеріал")]
    public int Id { get; set; }
    [Display(Name = "Курс")]
    public int CourceId { get; set; }
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Заголовок")]
    public string Title { get; set; } = null!;
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Посилання")]
    public string Link { get; set; } = null!;
    [Display(Name = "Курс")]
    public virtual Cource Cource { get; set; } = null!;
}
