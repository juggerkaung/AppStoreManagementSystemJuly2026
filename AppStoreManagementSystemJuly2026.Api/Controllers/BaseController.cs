
using AppStoreManagementSystem.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppStoreManagementSystemJuly2026.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        [NonAction]
        public IActionResult Execute(object data)
        {
            string json = JsonConvert.SerializeObject(data); // obj to json
            Result<object> result = JsonConvert.DeserializeObject<Result<object>>(json)!; // json to obj

            if (result.IsSuccess)
            {
                return Ok(data);
            }
            else
            {
                return StatusCode(400, data);
            }
        }
    }
}
