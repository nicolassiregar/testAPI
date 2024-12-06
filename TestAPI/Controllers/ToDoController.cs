using Microsoft.AspNetCore.Mvc;
using TestAPI.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using TestAPI.Repository;

namespace TestAPI.Controllers
{
    [ApiController]
    //[Route("controller")]
    public class ToDoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TestTable1Repository _repository;

        public ToDoController(IConfiguration configuration, TestTable1Repository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestModel request)
        {
            if (request.Username == "admin" && request.Password == "password")
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, request.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"])),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                );

                return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return Unauthorized();
        }

        [HttpGet("listProduk")]
        [Authorize]
        public IActionResult listProduk()
        {
            var data = _repository.produkRepository();
            return Ok(new { data = data });
        }

        [HttpGet("stock")]
        [Authorize]
        public IActionResult stock()
        {
            var data = _repository.checkStock();
            return Ok(new { data = data });
        }

        [HttpGet("reportPos")]
        [Authorize]
        public IActionResult reportPos()
        {
            var data = _repository.reportPos();
            return Ok(new { data = data });
        }

        [HttpPost("InsertPos")]
        [Authorize]
        public IActionResult InsertPos(List<penjualanParam> param)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTime = now.ToUnixTimeSeconds();
            string unixTimeString = unixTime.ToString();


            _repository.insertPos(param, unixTimeString);
            return Ok(new { status = "success" });
        }

        [HttpPost("UpdateItem")]
        [Authorize]
        public IActionResult UpdateItem(produkModel param)
        {
            var response = _repository.updateItem(param);
            return Ok(new { status = "success" });
        }
    }
}
