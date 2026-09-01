using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GenbaseGoldReports.Api.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                          ?? User.Identity?.Name
                                          ?? "System";
    }
}
