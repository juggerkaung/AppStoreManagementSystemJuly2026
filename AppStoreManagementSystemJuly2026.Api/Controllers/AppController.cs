using AppStoreManagementSystem.Database.AppDbContextModel;
using AppStoreManagementSystem.Domain.Features.Api.Features.App.Models;
using AppStoreManagementSystem.Domain.Features.App;
using AppStoreManagementSystem.Domain;

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
    }
}
