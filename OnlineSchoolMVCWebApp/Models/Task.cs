using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineSchoolMVCWebApp.Models;

public partial class Task
{
    public int Id { get; set; }
    [Display(Name = "Курс")]
    public int CourceId { get; set; }
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Заголовок")]
    public string Title { get; set; } = null!;
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Контент")]
    public string TaskContent { get; set; } = null!;
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Порядок сортування")]
    public int SortOrder { get; set; }

    public virtual Cource Cource { get; set; } = null!;
}
