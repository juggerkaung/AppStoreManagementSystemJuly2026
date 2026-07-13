using AppStoreManagementSystem.Database.AppDbContextModel;

namespace AppStoreManagementSystem.Domain.Features.Api.Features.App.Models;

public class AppListRequestModel
{

    public string? AppName { get; set; }

    public string? Description { get; set; }

    public string? Version { get; set; }

    public long? FileSize { get; set; }

    public int PageNo { get; set; }

    public int PageSize { get; set; }
}

public class AppListResponseModel
{
    public List<AppModel> Apps { get; set; } = new List<AppModel>();
   
}

//public class AppModel
//{

//    public int AppId { get; set; }

//    public string AppName { get; set; } = null!;

//    public string? Description { get; set; }

//    public string Version { get; set; } = null!;

//    public long FileSize { get; set; }

//    public string Status { get; set; } = null!;

//    public DateTime CreatedAt { get; set; }


//    public virtual TblAppCategory Category { get; set; } = null!;

//    public virtual ICollection<TblDownload> TblDownloads { get; set; } = new List<TblDownload>();
//}
public class AppModel
{
    public int AppId { get; set; }

    public string AppName { get; set; } = null!;

    public string? Description { get; set; }

    public string Version { get; set; } = null!;

    public long FileSize { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int DownloadCount { get; set; }

    public virtual TblAppCategory Category { get; set; } = null!;

    public virtual ICollection<TblDownload> TblDownloads { get; set; } = new List<TblDownload>();
}