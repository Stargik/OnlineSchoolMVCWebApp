using System;
using System.Collections.Generic;

namespace OnlineSchoolMVCWebApp.Models;

public partial class SubjectCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Cource> Cources { get; } = new List<Cource>();
}
