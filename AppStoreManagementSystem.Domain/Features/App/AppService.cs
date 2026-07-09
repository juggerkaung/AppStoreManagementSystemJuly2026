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
            .ToList();

            Result<AppListResponseModel> result = new Result<AppListResponseModel>
            {
                IsSuccess = true,
                Message = "Apps retrieved successfully.",
                Data = new AppListResponseModel
                {
                    Apps = apps.Select(x => new AppModel
                    {
                        AppId = x.AppId,
                        AppName = x.AppName,
                        Description = x.Description,
                        Version = x.Version,
                        FileSize = x.FileSize,
                        Status = x.Status,
                        CreatedAt = x.CreatedAt
                    }).ToList()
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

    public Result<int> AppCreate(AppCreateRequestModel request)
    {
        try
        {
            TblApp app = new TblApp
            {
                AppName = request.AppName,
                Description = request.Description,
                Version = request.Version,
                FileSize = request.FileSize,
                FilePath = request.FilePath,
                Status = request.Status,
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
