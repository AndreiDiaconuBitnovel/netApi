using System;
using System.Collections.Generic;

namespace WebApplication2.DataDB;

public partial class User
{
    public Guid IdUser { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public Guid? IdImg { get; set; }

    public virtual Image? IdImgNavigation { get; set; }
}
