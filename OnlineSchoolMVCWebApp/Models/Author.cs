using System;
using System.Collections.Generic;

namespace OnlineSchoolMVCWebApp.Models;

public partial class Author
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Cource> Cources { get; } = new List<Cource>();
}
