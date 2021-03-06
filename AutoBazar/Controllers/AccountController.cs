﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoBazar.BLL.Abstract;
using AutoBazar.BLL.DTO;
using AutoBazar.BLL.Services;
using AutoBazar.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<DbUser> _userManager;
        private readonly SignInManager<DbUser> _signInManager;
        private readonly IJWTTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly CancellationToken _cancellationToken;

        public AccountController(
            IWebHostEnvironment env,
            IConfiguration configuration,
            UserManager<DbUser> userManager,
            SignInManager<DbUser> signInManager,
            IJWTTokenService jWTTokenService,
            ApplicationDbContext context
            )
        {
            this._configuration = configuration;
            this._env = env;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._jwtTokenService = jWTTokenService;
            this._context = context;
        }
        //[HttpGet]
        ////getAll Users list
        //public IEnumerable<UserDTO> GetUsers()
        //{

        //    return new string[] { "User1", "User2" };
        //}

        //// GET user by Id
        //[HttpGet("{id}")]
        //public UserDTO Get(int id)
        //{
        //    return "Some User";
        //}

        // Login method
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginDTO loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new {
                    token = "",
                    invalid = "Не валідна модель" }); 
            }
            var result = await _signInManager
                .PasswordSignInAsync(loginModel.Email, loginModel.Password,
                false, false);

            if (!result.Succeeded)
            {
                return BadRequest(new {
                    token = "",
                    invalid = "Не правильно введені дані!" });
            }
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            await _signInManager.SignInAsync(user, isPersistent: false);
            var _refreshToken = AuthHelpers.GenerateRefreshToken();
            await _context.UserRefreshTokens.AddAsync(new UserRefreshToken
            {
                UserId = user.Id,
                RefreshToken = _refreshToken,
                ExpiryDate = DateTime.UtcNow.AddMonths(1)
            }, _cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync(_cancellationToken).ConfigureAwait(false);
            return Ok(
            new
            {
                token = _jwtTokenService.CreateToken(user),
                refreshToken = _refreshToken
            });
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