using System;
using System.Collections.Generic;

namespace OnlineSchoolMVCWebApp.Models;

public partial class Cource
{
    public int Id { get; set; }

    public int AuthorId { get; set; }

    public int SubjectCategoryId { get; set; }

    public int LevelId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual ICollection<Attachment> Attachments { get; } = new List<Attachment>();

    public virtual Author Author { get; set; } = null!;

    public virtual Level Level { get; set; } = null!;

    public virtual SubjectCategory SubjectCategory { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; } = new List<Task>();
}
