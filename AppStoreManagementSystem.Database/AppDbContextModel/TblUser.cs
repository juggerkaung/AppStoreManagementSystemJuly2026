using System;
using System.Collections.Generic;

namespace AppStoreManagementSystem.Database.AppDbContextModel;

public partial class TblUser
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDelete { get; set; }

    public virtual ICollection<TblDownload> TblDownloads { get; set; } = new List<TblDownload>();
}
