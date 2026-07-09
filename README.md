```
dotnet ef dbcontext scaffold "Server=.; Database=DotNetTraining;User Id=sa;Password=sasa@123;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models -c AppDbContext --no-onconfiguring -f
```
---
A. Create (POST )

1. Create request model in 
 Features/App/Models/AppCreateRequestModel.cs

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


---

2. 
Create AppCreate Method in AppService:

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
---

3. In Controller add this endpoint:
 
        [HttpPost]
        public IActionResult AppCreate(AppCreateRequestModel request)
        {
            var result = _appService.AppCreate(request);

            return Execute(result);
        }
---
2. Update (PUT)

First Create AppUpdateRequestModel.cs file:

a. 
namespace AppStoreManagementSystem.Domain.Features.App.Models
{
    public class AppUpdateRequestModel
    {
        public int AppId { get; set; }

        public string AppName { get; set; } = null!;

        public string? Description { get; set; }


        public string Version { get; set; } = null!;

        public string Status { get; set; } = null!;
    }
}

---
b. In AppService.cs file, add this :
 
 
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

---
c. Put the endpoint in AppController.cs file :

        [HttpPut]
        public IActionResult AppUpdate(AppUpdateRequestModel request)
        {
            var result = _appService.AppUpdate(request);

            return Execute(result);
        }

---

3. Delete (DELETE)

a. AppService.cs file: 


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

---

b. AppController.cs file:

       [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _appService.AppDelete(id);

            return Execute(result);
        }
---



