using System;
using System.Collections.Generic;

namespace OnlineSchoolMVCWebApp.Models;

public partial class Attachment
{
    public int Id { get; set; }

    public int CourceId { get; set; }

    public string Title { get; set; } = null!;

    public string Link { get; set; } = null!;

    public virtual Cource Cource { get; set; } = null!;
}
