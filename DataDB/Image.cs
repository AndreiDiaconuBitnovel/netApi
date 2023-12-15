using System;
using System.Collections.Generic;

namespace WebApplication2.DataDB;

public partial class Image
{
    public Guid Id { get; set; }

    public byte[] ImageObj { get; set; } = null!;
}
