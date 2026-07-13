using AppStoreManagementSystem.Database.AppDbContextModel;
using AppStoreManagementSystem.Domain;
using AppStoreManagementSystem.Domain.Features.Api.Features.App.Models;
using AppStoreManagementSystem.Domain.Features.App;
using AppStoreManagementSystem.Domain.Features.App.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppStoreManagementSystemJuly2026.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppController : BaseController
    {

        private readonly AppService _appService;

        public AppController(AppService appService)
        {
            _appService = appService;
        }

        [HttpGet]
        public IActionResult AppList([FromQuery] AppListRequestModel request)
        {
            var result = _appService.GetList(request);
            //if (result.IsSuccess)
            //{
            //    return Ok(result.Data);
            //}
            //return BadRequest(result.Message);

            return Execute(result);
        }

        [HttpGet("categories")]
        public IActionResult CategoryList([FromQuery] bool includeInactive = false)
        {
            var result = _appService.GetCategoryList(includeInactive);

            return Execute(result);
        }

        [HttpPost("categories")]
        public IActionResult CategoryCreate(AppCategoryCreateRequestModel request)
        {
            var result = _appService.CategoryCreate(request);

            return Execute(result);
        }

        [HttpPut("categories")]
        public IActionResult CategoryUpdate(AppCategoryUpdateRequestModel request)
        {
            var result = _appService.CategoryUpdate(request);

            return Execute(result);
        }

        [HttpPost]
        public IActionResult AppCreate(AppCreateRequestModel request)
        {
            var result = _appService.AppCreate(request);

            return Execute(result);
        }

        [HttpPut]
        public IActionResult AppUpdate(AppUpdateRequestModel request)
        {
            var result = _appService.AppUpdate(request);

            return Execute(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _appService.AppDelete(id);

            return Execute(result);
        }


    }
}
