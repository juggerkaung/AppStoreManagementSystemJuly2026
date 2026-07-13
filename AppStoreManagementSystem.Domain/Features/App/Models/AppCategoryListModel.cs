namespace AppStoreManagementSystem.Domain.Features.App.Models;

public class AppCategoryListModel
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDelete { get; set; }
}

public class AppCategoryCreateRequestModel
{
    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }
}

public class AppCategoryUpdateRequestModel
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }
}
