using System;
using System.Collections.Generic;

namespace OnlineSchoolMVCWebApp.Models;

public partial class Level
{
    public int Id { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Cource> Cources { get; } = new List<Cource>();
}
