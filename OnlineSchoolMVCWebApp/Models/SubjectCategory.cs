using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineSchoolMVCWebApp.Models;

public partial class SubjectCategory
{
    [Display(Name = "Категорія")]
    public int Id { get; set; }
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Заголовок")]
    public string Name { get; set; } = null!;

    public virtual ICollection<Cource> Cources { get; } = new List<Cource>();
}
