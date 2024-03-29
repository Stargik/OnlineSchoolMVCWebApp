﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineSchoolMVCWebApp.Models;

public partial class Cource
{
    [Display(Name = "Курс")]
    public int Id { get; set; }
    [Display(Name = "Автор")]
    public int AuthorId { get; set; }
    [Display(Name = "Категорія")]
    public int SubjectCategoryId { get; set; }
    [Display(Name = "Рівень складності")]
    public int LevelId { get; set; }
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Назва")]
    public string Title { get; set; } = null!;
    [Display(Name = "Короткий опис")]
    public string? Description { get; set; }
    [Display(Name = "Дата створення")]
    public DateTime CreationDate { get; set; }
    [Display(Name = "Додаткові матеріали")]
    public virtual ICollection<Attachment> Attachments { get; } = new List<Attachment>();
    [Display(Name = "Автор")]
    public virtual Author Author { get; set; } = null!;
    [Display(Name = "Рівень складності")]
    public virtual Level Level { get; set; } = null!;
    [Display(Name = "Категорія")]
    public virtual SubjectCategory SubjectCategory { get; set; } = null!;
    [Display(Name = "Завдання")]
    public virtual ICollection<Task> Tasks { get; } = new List<Task>();
}
