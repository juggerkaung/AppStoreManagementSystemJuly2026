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
            var apps = _db.TblApps
            .AsNoTracking()
            .Where(x => x.Status == "Active")
            .OrderByDescending(x=> x.AppId)
            .Skip((request.PageNo - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new AppModel
            {
                AppId = x.AppId,
                AppName = x.AppName,
                Description = x.Description,
                Version = x.Version,
                FileSize = x.FileSize,
                Status = x.Status,
                CreatedAt = x.CreatedAt,
                CategoryName = _db.TblAppCategories
                    .Where(c => c.CategoryId == x.CategoryId && c.IsActive && !c.IsDelete)
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

    public Result<List<AppCategoryListModel>> GetCategoryList()
    {
        try
        {
            var categories = _db.TblAppCategories
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDelete)
                .OrderBy(x => x.CategoryName)
                .Select(x => new AppCategoryListModel
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName
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
                .FirstOrDefault(x => x.AppId == request.AppId);


            if (app == null)
            {
                return new Result<int>
                {
                    IsSuccess = false,
                    Message = "App not found."
                };
            }


            app.AppName = request.AppName;
            app.Description = request.Description;
            app.Version = request.Version;
            app.Status = request.Status;


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
