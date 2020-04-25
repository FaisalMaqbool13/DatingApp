using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _Config;

        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _Config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDtos regDts)
        {
            regDts.UserName = regDts.UserName.ToLower();

            if (await _repo.UserExists(regDts.UserName))
                return BadRequest("User already exist");

            var UserToCreate = new User
            {
                Username = regDts.UserName,
            };

            var CreateUser = await _repo.Register(UserToCreate, regDts.Password);

            return StatusCode(201);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> login(UserLoginDtos userforlogin)
        {
            var userfromrepo = await _repo.Login(userforlogin.UserName, userforlogin.Password);

            if (userfromrepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userfromrepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userfromrepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Config.GetSection("AppSetting:Token").Value));

            var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var tokendescriptor= new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity(claims),
                Expires=DateTime.Now.AddDays(1),
                SigningCredentials=creds
            };

            var tokenhandler=new JwtSecurityTokenHandler();
            var token= tokenhandler.CreateToken(tokendescriptor);

            return Ok(new {
                token =tokenhandler.WriteToken(token)
            });

        }

    }
}