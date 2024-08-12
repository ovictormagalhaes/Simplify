using Microsoft.AspNetCore.Mvc;
using Simplify.Examples.API.Controllers.Requests;
using Simplify.Examples.API.Queries;

namespace Simplify.Examples.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController(IUserQuery userQuery) : ControllerBase
    {
        private readonly IUserQuery _userQuery = userQuery;

        [HttpGet]
        public IActionResult GetByFilter([FromQuery] GetUserFilterRequest request)
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            return Ok();
        }
    }
}
