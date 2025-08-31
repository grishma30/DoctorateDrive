using System;
using System.Collections.Generic;

namespace DoctorateDrive.Models;

public partial class Document
{
    public int DocumentId { get; set; }

    public int StudentId { get; set; }

    public string DocumentType { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public DateTime UploadedDate { get; set; }

    public virtual StudentDetail DocumentNavigation { get; set; } = null!;
}
