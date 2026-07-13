using AppStoreManagementSystem.Database.AppDbContextModel;
using AppStoreManagementSystem.Domain.Features.Api.Features.App.Models;
using AppStoreManagementSystem.Domain.Features.App.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppStoreManagementSystem.Domain.Features.App;

public class AppService
{
    private readonly AppDbContext _db;

    public AppService(AppDbContext db)
    {
        _db = db;
    }

    public Result<AppListResponseModel> GetList(AppListRequestModel request)
    {
        try
        {
            var query = _db.TblApps
                .AsNoTracking()
                .Where(x => !x.IsDelete);

            if (!request.IncludeInactive)
            {
                query = query.Where(x => x.Status == "Active");
            }

            var apps = query
                .OrderByDescending(x => x.AppId)
                .Skip((request.PageNo - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AppModel
                {
                    AppId = x.AppId,
                    AppName = x.AppName,
                    Description = x.Description,
                    CategoryId = x.CategoryId,
                    Version = x.Version,
                    FilePath = x.FilePath,
                    FileSize = x.FileSize,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    IsDelete = x.IsDelete,
                    CategoryName = _db.TblAppCategories
                        .Where(c => c.CategoryId == x.CategoryId && !c.IsDelete)
                        .Select(c => c.CategoryName)
                        .FirstOrDefault() ?? string.Empty,
                    DownloadCount = _db.TblDownloads.Count(d => d.AppId == x.AppId)
                })
                .ToList();

            Result<AppListResponseModel> result = new Result<AppListResponseModel>
            {
                IsSuccess = true,
                Message = "Apps retrieved successfully.",
                Data = new AppListResponseModel
                {
                    Apps = apps
                }
            };

            return result;
        }
        catch (Exception ex)
        {

            Result<AppListResponseModel> result = new Result<AppListResponseModel>
            {
                IsSuccess = false,
                Message = ex.Message

            };

            return result; // 400
        }

    }

    public Result<List<AppCategoryListModel>> GetCategoryList(bool includeInactive = false)
    {
        try
        {
            var query = _db.TblAppCategories
                .AsNoTracking()
                .Where(x => !x.IsDelete);

            if (!includeInactive)
            {
                query = query.Where(x => x.IsActive);
            }

            var categories = query
                .OrderBy(x => x.CategoryName)
                .Select(x => new AppCategoryListModel
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    IsDelete = x.IsDelete
                })
                .ToList();

            return new Result<List<AppCategoryListModel>>
            {
                IsSuccess = true,
                Message = "Categories retrieved successfully.",
                Data = categories
            };
        }
        catch (Exception ex)
        {
            return new Result<List<AppCategoryListModel>>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public Result<int> AppCreate(AppCreateRequestModel request)
    {
        try
        {
            bool categoryExists = _db.TblAppCategories
                .AsNoTracking()
                .Any(x => x.CategoryId == request.CategoryId && x.IsActive && !x.IsDelete);

            if (!categoryExists)
            {
                return new Result<int>
                {
                    IsSuccess = false,
                    Message = "Selected category is not available."
                };
            }

            TblApp app = new TblApp
            {
                AppName = request.AppName,
                Description = request.Description,
                Version = request.Version,
                FileSize = request.FileSize,
                FilePath = request.FilePath,
                Status = "Active",
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.Now
            };


            _db.TblApps.Add(app);

            int result = _db.SaveChanges();


            return new Result<int>
            {
                IsSuccess = true,
                Message = "App created successfully.",
                Data = app.AppId
            };

        }
        catch (Exception ex)
        {
            return new Result<int>
            {
                IsSuccess = false,
                //Message = ex.Message
                Message = ex.InnerException?.Message ?? ex.Message

            };
        }
    }

    public Result<int> AppUpdate(AppUpdateRequestModel request)
    {
        try
        {
            var app = _db.TblApps
                .FirstOrDefault(x => x.AppId == request.AppId && !x.IsDelete);


            if (app == null)
            {
                return new Result<int>
                {
                    IsSuccess = false,
                    Message = "App not found."
                };
            }

            bool categoryExists = _db.TblAppCategories
                .AsNoTracking()
                .Any(x => x.CategoryId == request.CategoryId && x.IsActive && !x.IsDelete);

            if (!categoryExists)
            {
                return new Result<int>
                {
                    IsSuccess = false,
                    Message = "Selected category is not available."
                };
            }

            if (!IsVersionGreater(request.Version, app.Version))
            {
                return new Result<int>
                {
                    IsSuccess = false,
                    Message = $"Version must be greater than current version ({app.Version})."
                };
            }

            app.AppName = request.AppName;
            app.Description = request.Description;
            app.Version = request.Version;
            app.FileSize = request.FileSize;
            app.Status = request.Status;
            app.CategoryId = request.CategoryId;
            app.IsDelete = request.IsDelete;


            _db.SaveChanges();


            return new Result<int>
            {
                IsSuccess = true,
                Message = "App updated successfully.",
                Data = app.AppId
            };
        }
        catch (Exception ex)
        {
            return new Result<int>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public Result<int> CategoryCreate(AppCategoryCreateRequestModel request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.CategoryName))
            {
                return new Result<int>
                {
                    IsSuccess = false,
                    Message = "Category name is required."
                };
            }

            bool exists = _db.TblAppCategories
                .AsNoTracking()
                .Any(x => x.CategoryName == request.CategoryName && !x.IsDelete);

            if (exists)
            {
                return new Result<int>
                {
                    IsSuccess = false,
                    Message = "Category name already exists."
                };
            }

            TblAppCategory category = new TblAppCategory
            {
                CategoryName = request.CategoryName,
                Description = request.Description,
                IsActive = true,
                CreatedAt = DateTime.Now,
                IsDelete = false
            };

            _db.TblAppCategories.Add(category);
            _db.SaveChanges();

            return new Result<int>
            {
                IsSuccess = true,
                Message = "Category added successfully.",
                Data = category.CategoryId
            };
        }
        catch (Exception ex)
        {
            return new Result<int>
            {
                IsSuccess = false,
                Message = ex.InnerException?.Message ?? ex.Message
            };
        }
    }

    public Result<int> CategoryUpdate(AppCategoryUpdateRequestModel request)
    {
        try
        {
            var category = _db.TblAppCategories
                .FirstOrDefault(x => x.CategoryId == request.CategoryId && !x.IsDelete);

            if (category == null)
            {
                return new Result<int>
                {
                    IsSuccess = false,
                    Message = "Category not found."
                };
            }

            if (string.IsNullOrWhiteSpace(request.CategoryName))
            {
                return new Result<int>
                {
                    IsSuccess = false,
                    Message = "Category name is required."
                };
            }

            bool exists = _db.TblAppCategories
                .AsNoTracking()
                .Any(x => x.CategoryId != request.CategoryId && x.CategoryName == request.CategoryName && !x.IsDelete);

            if (exists)
            {
                return new Result<int>
                {
                    IsSuccess = false,
                    Message = "Category name already exists."
                };
            }

            category.CategoryName = request.CategoryName;
            category.Description = request.Description;
            category.IsActive = request.IsActive;
            category.IsDelete = request.IsDelete;

            _db.SaveChanges();

            return new Result<int>
            {
                IsSuccess = true,
                Message = "Category updated successfully.",
                Data = category.CategoryId
            };
        }
        catch (Exception ex)
        {
            return new Result<int>
            {
                IsSuccess = false,
                Message = ex.InnerException?.Message ?? ex.Message
            };
        }
    }

    private static bool IsVersionGreater(string newVersion, string currentVersion)
    {
        var newParts = ParseVersion(newVersion);
        var currentParts = ParseVersion(currentVersion);
        int maxLength = Math.Max(newParts.Count, currentParts.Count);

        for (int i = 0; i < maxLength; i++)
        {
            int newPart = i < newParts.Count ? newParts[i] : 0;
            int currentPart = i < currentParts.Count ? currentParts[i] : 0;

            if (newPart > currentPart) return true;
            if (newPart < currentPart) return false;
        }

        return false;
    }

    private static List<int> ParseVersion(string version)
    {
        return version
            .Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => int.TryParse(x, out int value) ? value : -1)
            .ToList();
    }


    public Result<int> AppDelete(int id)
    {
        var app = _db.TblApps
            .FirstOrDefault(x => x.AppId == id);


        if (app == null)
        {
            return new Result<int>
            {
                IsSuccess = false,
                Message = "App not found."
            };
        }


        app.Status = "Inactive";


        _db.SaveChanges();


        return new Result<int>
        {
            IsSuccess = true,
            Message = "App deleted successfully."
        };
    }

}
