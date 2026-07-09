using System;
using System.Collections.Generic;

namespace AppStoreManagementSystem.Database.AppDbContextModel;

public partial class TblApp
{
    public int AppId { get; set; }

    public string AppName { get; set; } = null!;

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public string Version { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public long FileSize { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsDelete { get; set; }

    public virtual TblAppCategory Category { get; set; } = null!;

    public virtual ICollection<TblDownload> TblDownloads { get; set; } = new List<TblDownload>();
}
