using System;
using System.Collections.Generic;

namespace WebApplication2.DataDB;

public partial class TranslateLanguage
{
    public string Code { get; set; } = null!;

    public string? NameInternational { get; set; }

    public string? NameLocal { get; set; }
}
