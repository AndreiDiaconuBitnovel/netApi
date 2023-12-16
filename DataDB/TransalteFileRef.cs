using System;
using System.Collections.Generic;

namespace WebApplication2.DataDB;

public partial class TransalteFileRef
{
    public Guid UserId { get; set; }

    public string Source { get; set; } = null!;

    public string Target { get; set; } = null!;

    public string InputFile { get; set; } = null!;

    public string OutputFile { get; set; } = null!;

    public Guid Id { get; set; }
}
