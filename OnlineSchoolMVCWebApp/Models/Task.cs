using System;
using System.Collections.Generic;

namespace OnlineSchoolMVCWebApp.Models;

public partial class Task
{
    public int Id { get; set; }

    public int CourceId { get; set; }

    public string Title { get; set; } = null!;

    public string TaskContent { get; set; } = null!;

    public int SortOrder { get; set; }

    public virtual Cource Cource { get; set; } = null!;
}
