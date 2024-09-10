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
        public IActionResult GetPagedByFilter([FromQuery] GetUserFilterRequest request)
        {
            return Ok();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var user = await _userQuery.FirstOrDefaultByIdAsync(id);

            if (user == null)
                return NotFound($"Not found user with id {id}");

            return Ok(user);
        }
    }
}
