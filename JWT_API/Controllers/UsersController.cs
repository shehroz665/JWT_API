

using Azure;
using JWT_API.Data;
using JWT_API.Logging;
using JWT_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT_API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationContext _db;
        private readonly LoggingInterface _logging;
        public UsersController(IConfiguration configuration,ApplicationContext db,LoggingInterface logging)
        {
            _configuration=configuration;
            _db=db;
            _logging=logging;
        }

        [AllowAnonymous]
        [HttpPost("/login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Users> login([FromBody] Users userObj)
        {
            if (userObj==null)
            {
                return BadRequest(userObj);
            }
            var user= _db.User.FirstOrDefault(x=>x.Email==userObj.Email && x.Password==userObj.Password);
            if (user!=null)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub,_configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString())
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: signIn

                    );
                userObj.UserMessage="Login Successful";
                userObj.UserToken=new JwtSecurityTokenHandler().WriteToken(token);
                var response = _logging.Success("Login Successful", 200, userObj);
                return Content(response,"application/json");
            }
            else
            {
                var response = _logging.Failure("Login Failure: Please Enter right cridentials", 404,null);
                return Content(response, "application/json");
            }
        }

        [HttpPost("/signUp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Users> signUp([FromBody] Users userObj)
        {
            var response = "";
            if(userObj == null) {
                 response = _logging.Failure("Request is null", 404, null);
                return Content(response, "application/json");
            }
            var user= _db.User.FirstOrDefault(x=> x.Email==userObj.Email);
            if(user != null) {
                response = _logging.Failure("Email Already exists", 404, null);
                return Content(response, "application/json");
            }
            Users model = new()
            {
                Email=userObj.Email,
                Password=userObj.Password,
                UserMessage=" ",
                UserToken=" "
            };  
            _db.User.Add(model);
            _db.SaveChanges();
            response = _logging.Success("User Created Successfully", 201, model);
            return Content(response, "application/json");
        }

        [HttpGet("/allUsers")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Users> getAllUsers()
        {
            var user = _db.User.ToList();
            var response = _logging.Success("Users Fetched Successfully", 200, user);
            return Content(response, "application/json");
        }



        [HttpGet("/user/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Users> GetUserById(int id)
        {
            var response = "";
            var user = _db.User.FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                response = _logging.Failure("Result not found", 404, null);
                return Content(response, "application/json");
            }

            response = _logging.Success("User Fetched Successfully", 200, user);
            return Content(response, "application/json");
        }



    }
}
