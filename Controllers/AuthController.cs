using LTMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace LTMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LtmsContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthController(LtmsContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Compte>> Register(Compte request,string Password)
        {
            var existingUser = await _dbContext.Comptes.FirstOrDefaultAsync(c => c.Login == request.Login);

            if (existingUser != null)
            {
                return BadRequest("User already exists");
            }

            CreatePasswordHash(Password, out byte[] passwordHash, out byte[] passwordSalt);

            var compte = new Compte();

            compte.Login= request.Login;
            compte.Prenom= request.Prenom;
            compte.DateDeNaissance= request.DateDeNaissance;
            compte.Role= request.Role;
            compte.Nom= request.Nom;
            compte.Matricule= request.Matricule;
            compte.Tel= request.Tel;
         
            compte.PasswordHash = passwordHash;
            compte.PasswordSalt = passwordSalt;
               

            await _dbContext.Comptes.AddAsync(compte);
            await _dbContext.SaveChangesAsync();

            return Ok(compte);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Compte>> Login(Compte request, string Password)
        {
            var login = new Compte { Login = request.Login };
            var compte = await _dbContext.Comptes.FirstOrDefaultAsync(c => c.Login == login.Login);

            if (compte == null)
            {
                return BadRequest("le Compte n'existe pas");
            }

            if (!VerifyPasswordHash(Password, compte.PasswordHash, compte.PasswordSalt))
            {
                return BadRequest("mot de passe erroné");
            }

            var token = CreateToken(compte);
            return Ok(new { message = "Login successful", token, role = compte.Role });
        }

       
        private string CreateToken(Compte compte)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, compte.Login)
                // Add other claims as needed
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string GenerateRandomPassword(int length = 8)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            var random = new Random();

            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [Route("~/api/forgetpassword")]
        [HttpPost]
        public async Task<Object> ForgotPassword(MyObject email)
        {


            var user = await _dbContext.Comptes.FirstOrDefaultAsync(c => c.Login == email.email);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var newPassword = GenerateRandomPassword();
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

            var compteHash = await _dbContext.Comptes.FirstOrDefaultAsync(ch => ch.Login == user.Login);
            if (compteHash == null)
            {
                compteHash = new Compte();


                compteHash.PasswordHash = passwordHash;
                compteHash.PasswordSalt = passwordSalt;

                await _dbContext.Comptes.AddAsync(compteHash);
            }
            else
            {
                compteHash.PasswordHash = passwordHash;
                compteHash.PasswordSalt = passwordSalt;
            }

            await _dbContext.SaveChangesAsync();

            SendEmail(user.Login, newPassword);

            // Return a success response
            return Ok(new { message = "Password reset email sent" });
        }




        private void SendEmail(string mail, string newPassword)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("mayssaneji6@gmail.com"));
            email.To.Add(MailboxAddress.Parse(mail));
            email.Subject = ("Password Reset");
            email.Body = new TextPart(TextFormat.Html) { Text = "Your new password is: " + newPassword };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp-relay.sendinblue.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("mayssaneji6@gmail.com", "n4aLdtYXPjvgWTD3");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
