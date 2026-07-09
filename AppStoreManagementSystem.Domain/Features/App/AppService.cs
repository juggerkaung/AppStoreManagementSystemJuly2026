using AppStoreManagementSystem.Database.AppDbContextModel;
using AppStoreManagementSystem.Domain.Features.Api.Features.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


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
}
