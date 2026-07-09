using System;
using System.Collections.Generic;

namespace AppStoreManagementSystem.Database.AppDbContextModel;

public partial class TblDownload
{
    public int DownloadId { get; set; }

    public int UserId { get; set; }

    public int AppId { get; set; }

    public DateTime DownloadDate { get; set; }

    public bool IsDelete { get; set; }

    public virtual TblApp App { get; set; } = null!;

    public virtual TblUser User { get; set; } = null!;
}
