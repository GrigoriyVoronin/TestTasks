using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABTestReal.Services;
using DbModels;
using DbRepositories;

namespace ABTestReal.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _usersService;

        public UsersController(UsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            return Ok(await _usersService.GetUsersAsync());
        }

        [HttpPost]
        public async Task<ActionResult<List<User>>> UpdateUsers([FromBody] List<User> users)
        {
            return Accepted(await _usersService.UpdateUsersAsync(users));
        }
    }
}
