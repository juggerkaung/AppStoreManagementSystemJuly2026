using System;
using System.Collections.Generic;

namespace AppStoreManagementSystem.Database.AppDbContextModel;

public partial class TblAppCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDelete { get; set; }

    public virtual ICollection<TblApp> TblApps { get; set; } = new List<TblApp>();
}
