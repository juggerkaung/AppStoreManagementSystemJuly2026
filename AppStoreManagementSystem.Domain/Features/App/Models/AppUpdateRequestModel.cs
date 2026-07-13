using System.ComponentModel.DataAnnotations;

namespace AppStoreManagementSystem.Domain.Features.App.Models;

public class AppUpdateRequestModel
{
    public int AppId { get; set; }

    [Required]
    public string AppName { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    public string Version { get; set; } = null!;

    [Range(1, long.MaxValue)]
    public long FileSize { get; set; }

    [Required]
    public string Status { get; set; } = "Active";

    [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
    public int CategoryId { get; set; }

    public bool IsDelete { get; set; }
}
