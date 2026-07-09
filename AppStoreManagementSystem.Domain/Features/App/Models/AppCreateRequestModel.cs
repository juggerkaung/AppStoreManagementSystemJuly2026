using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppStoreManagementSystem.Domain.Features.App.Models;

public class AppCreateRequestModel
{
    public string AppName { get; set; } = null!;

    public string? Description { get; set; }

    public string Version { get; set; } = null!;

    public long FileSize { get; set; }

    public string FilePath { get; set; } = null!;


    public string Status { get; set; } = "Active";

    public int CategoryId { get; set; }
}
