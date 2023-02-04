using Microsoft.AspNetCore.Mvc;
using DynamicRazorRender.Shared;
using DynamicRazorRender.Server.Filters;
using DynamicRazorRender.Server.Events;

namespace DynamicRazorRender.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ApiResultWrapFilter]
    public class HealthController : ControllerBase
    {

        public HealthController()
        {
            
        }

        [HttpGet]
        public void Check(){
            
        }
    }
}