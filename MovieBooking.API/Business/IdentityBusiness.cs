using Microsoft.IdentityModel.Tokens;
using MovieBooking.API.Interfaces.IBusiness;
using MovieBooking.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MovieBooking.API.Business
{
    public class IdentityBusiness : IIdentityBusiness
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<IdentityBusiness> _logger;

        public IdentityBusiness(IConfiguration configuration, ILogger<IdentityBusiness> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void CreatePasswordHashSalt(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            _logger.LogInformation("creating passwordHashsalt : Identity Business");

            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool AuthorizeUser(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            _logger.LogInformation("Authorizing user : Identity Business");

            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

       
        public string CreateToken(User user)
        {
            _logger.LogInformation("creating user token : Identity Business");

            string token = string.Empty;


            if (user.LoginId == "admin" && user.Password == "admin@123")
            {
                List<Claim> claims = new List<Claim>
                {
                new Claim("UserName", user.LoginId),
                new Claim("Password", user.Password),
                new Claim("Role","admin"),
                new Claim(ClaimTypes.Role,"admin")

                };


                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
               _configuration.GetSection("AppSettings:Token").Value));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

                var jwt = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(20),
                    signingCredentials: creds);

                token = new JwtSecurityTokenHandler().WriteToken(jwt);

                return token;
            }
            else
            {
                List<Claim> claims = new List<Claim>
                {
                new Claim("UserName", user.LoginId),
                new Claim("Password", user.Password),
                new Claim("Role","users"),
                new Claim(ClaimTypes.Role,"users")
                
                };



                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
               _configuration.GetSection("AppSettings:Token").Value));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

                var jwt = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(20),
                    signingCredentials: creds);

                token = new JwtSecurityTokenHandler().WriteToken(jwt);

                return token;
            }






               
        }
    }
}
