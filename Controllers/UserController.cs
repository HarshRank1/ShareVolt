using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShareVolt.Models;
using ShareVolt.Repositories;
using ShareVolt.Services;
using ShareVolt.ViewModels;
using System.Linq;

namespace ShareVolt.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly AuthService _authService;


        public UserController(IUserRepository userRepo, AuthService authService )
        {
            _userRepo = userRepo;
            _authService = authService;
        }

        public IActionResult Index()
        {
            var model = _userRepo.GetAllUsers();
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        public IActionResult Create(User model)
        {
            if (ModelState.IsValid)
            {
                User newUser = new User ()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password,
                    //Password = _authService.HashPassword(new User(), model.Password),
                    WalletBalance = model.WalletBalance,
                    IsAdmin = model.IsAdmin
                };
                _userRepo.AddUser(newUser);
                return  RedirectToAction("details", new {id = newUser.Id} );
            }
            return View(model);
        }

        [Authorize]
        public ViewResult Details(int id)
        {
            User user = _userRepo.GetUser(id);
            if(user == null)
            {
                Response.StatusCode = 404;
                return View("UserNotFound", id);
            }
            return View(user);
        }


        //[Authorize]
        //[HttpGet]
        //public IActionResult Edit(int id) 
        //{
        //    User userFetched = _userRepo.GetUser(id);
        //    User newUser = new User
        //    {
        //        Name = userFetched.Name,
        //        Email = userFetched.Email,
        //        Password = userFetched.Password,
        //        WalletBalance = userFetched.WalletBalance,
        //        IsAdmin = userFetched.IsAdmin
        //    };
        //    return View(newUser);
        //}



        //[Authorize]
        //[HttpPost]
        //public IActionResult Edit(User model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        User userModel = _userRepo.GetUser(model.Id);
        //        userModel.Name = model.Name;
        //        userModel.Email = model.Email;
        //        userModel.Password = model.Password;
        //        userModel.WalletBalance = model.WalletBalance;
        //        userModel.IsAdmin = model.IsAdmin;

        //        User updatedUser = _userRepo.UpdateUser(userModel);

        //        return RedirectToAction("Index");
        //    }
        //    return View(model);
        //}

        [Authorize]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _userRepo.GetUser(id);

            if (user == null) return NotFound();

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Password = ""
            };

            return View(viewModel);
        }


        [Authorize]
        [HttpPost]
        public IActionResult Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepo.GetUser(model.Id);
                if (user == null) return NotFound();

                user.Name = model.Name;
                user.Email = model.Email;
                var userId = model.Id;

                user.Password = _authService.HashPassword(new User(), model.Password);

                var updatedUser = _userRepo.UpdateUser(user);

                return RedirectToAction("Details", "User", new { id = userId });

            }

            return View(model);
        }




        [Authorize]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            User user = _userRepo.GetUser(id);
            if(user == null)
            {
                return NotFound();
            }
            return View(user);
        }


        [Authorize]
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _userRepo.GetUser(id);
            _userRepo.DeleteUser(user.Id);

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Revenue()
        {
            var admin = _userRepo.GetAllUsers().FirstOrDefault(u => u.IsAdmin);
            if (admin == null)
            {
                return NotFound("Admin user not found.");
            }
            ViewBag.Revenue = admin.WalletBalance;
            ViewBag.Turnover = admin.TurnOver;
            return View();
        }



    }
}
