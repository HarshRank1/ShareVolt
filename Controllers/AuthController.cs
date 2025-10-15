using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using ShareVolt.DTOs;
using ShareVolt.Models;
using ShareVolt.Services;
using System.Linq;

namespace ShareVolt.Controllers
{
    public class AuthController : Controller
    {
        private readonly ShareVoltDbContext _context;
        private readonly AuthService _authService;
        private readonly JwtService _jwtService;

        public AuthController(ShareVoltDbContext context, AuthService authService, JwtService jwtService)
        {
            _context = context;
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Signup(SignupDto dto)
        {
            if (!ModelState.IsValid)
            {
               return View(dto);
            }
            if (_context.Users.Any(u => u.Email == dto.Email))
            {
                //return BadRequest("Email already registered!");
                ModelState.AddModelError("Email", "Email already registered!");
                return View(dto);
            }

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = _authService.HashPassword(new User(), dto.Password),
                WalletBalance = 100,
                IsAdmin = false
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            //return Ok("Signup successful!");
            return RedirectToAction("Login", "Auth");

        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null )
            {
                return NotFound("Account Not Found!");
            }
            if(!_authService.VerifyPassword(user, dto.Password))
            {
                return Unauthorized("Incorrect Password!");
            }
            var token = _jwtService.GenerateJwtToken(user);

            // we stored token in cookie following block is to send token to user itself
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // only http i did
                SameSite = SameSiteMode.Strict
            });

            //return Ok(new {token});
            return RedirectToAction("Index", "Charger");

        }


        [HttpGet]
        public IActionResult Logout()
        {
            // Remove the JWT cookie
            if (Request.Cookies.ContainsKey("jwt"))
            {
                Response.Cookies.Delete("jwt");
            }

            return RedirectToAction("Index", "Charger");
        }


    }
}
