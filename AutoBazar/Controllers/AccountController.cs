using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpGet]
        //getAll Users list
        public IEnumerable<UserDTO> GetUsers()
        {

            return new string[] { "User1", "User2" };
        }

        // GET user by Id
        [HttpGet("{id}")]
        public UserDTO Get(int id)
        {
            return "Some User";
        }

        // Login method
        [HttpPost]
        public void Login([FromBody] string value)
        {

        }

        // Regictration method
        [HttpPost]
        public void Registration([FromBody] string value)
        {

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void EditUser(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void DeleteUser(int id)
        {
        }
    }
}