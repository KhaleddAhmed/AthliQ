using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AthliQ.Admin.API.Controllers
{
    [Route("admin/[controller]")]
    [ApiController]
    public class BaseAPIController : ControllerBase { }
}
