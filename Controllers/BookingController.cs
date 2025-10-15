using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShareVolt.Models;
using ShareVolt.Repositories;
using ShareVolt.Services;
using ShareVolt.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using ShareVolt.DTOs;
using Razorpay.Api;

namespace ShareVolt.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IChargerRepository _chargerRepo;
        private readonly IUserRepository _userRepo;
        private readonly BookingService _bookingService;
        private readonly RazorpaySettingsService _razorpaySettingsService;

        public BookingController(IBookingRepository bookingRepo, IChargerRepository chargerRepo, IUserRepository userRepo , BookingService bookingService, RazorpaySettingsService razorpaySettingsService)
        {
            _bookingRepo = bookingRepo;
            _chargerRepo = chargerRepo;
            _userRepo = userRepo;
            _bookingService = bookingService;
            _razorpaySettingsService = razorpaySettingsService;
        }


        public IActionResult Index()
        {
            var model = _bookingRepo.GetAllBookings();
            return View(model);
        }

        public IActionResult UsersBookingIndex(int id)
        {
            var model = _bookingRepo.GetBookingByUserId(id);

            var viewModelList = model.Select(b => new BookingByUserListViewModel
            {
                Id = b.Id,
                StartTime = b.StartTime,
                Status = b.Status.ToString(),
                UserId = b.UserId,
                User = b.User,
                ChargerId = b.ChargerId,
                Charger = b.Charger
            }).ToList();
            return View(viewModelList);
        }

        public IActionResult ChargersBookingIndex(int id)
        {
            var model = _bookingRepo.GetBookingByChargerId(id);

            var viewModelList = model.Select(b => new BookingByChargerListViewModel
            {
                Id = b.Id,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                HostEarnings = b.HostEarnings,
                Status = b.Status.ToString(),
                UserId = b.UserId,
                User = b.User,
                ChargerId = b.ChargerId,
                Charger = b.Charger
            }).ToList();

            return View(viewModelList);
        }

        //[HttpGet]
        [HttpGet("Booking/Create/{id}")]
        public IActionResult Create(int id)
        {
            var charger = _chargerRepo.GetCharger(id);
            if (charger == null)
            {
                Response.StatusCode = 404;
                return View("ChargerNotFound", id);
            }
            var model = new BookingCreateViewModel
            {
                ChargerId = id,
                PricePerKwh = charger.PricePerKWh,
                BatteryOptions = BatteryConfig.BatteryOptions,
                Date = DateTime.Today,
                StartTime = DateTime.Now.TimeOfDay
            };
            return View(model);
        }

   



        [HttpPost]

        public IActionResult Create(BookingCreateViewModel model)
        {
            model.BatteryOptions = model.BatteryOptions ?? BatteryConfig.BatteryOptions;

            if (!ModelState.IsValid)
                return View(model);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            int currentUserId = int.Parse(userIdClaim);

            var charger = _chargerRepo.GetCharger(model.ChargerId);
            if (charger == null)
            {
                ModelState.AddModelError("", "Selected charger not found.");
                return View(model);
            }

            DateTime startDateTime = model.Date.Date.Add(model.StartTime);

            DateTime endTime = _bookingService.CalculateEndTime(model.BatteryCapacity, model.ExpectedBatteryPercentage, charger.ChargerSpeed, startDateTime);

            var booking = new Booking
            {
                StartTime = startDateTime,
                EndTime = endTime,
                ChargerId = model.ChargerId,
                SizeType = model.SizeType,
                BatterySize = model.BatteryCapacity,
                ExpectedBatteryPercentage = model.ExpectedBatteryPercentage,
                Status = BookingStatus.Cancelled,
                UserId = currentUserId
            };

            
            

            // Price calc
            decimal unitsConsumed = (decimal)(endTime - startDateTime).TotalHours * charger.ChargerSpeed;
            _bookingService.CalculateAmounts(booking, model.PricePerKwh, unitsConsumed);
                    //data added
            _bookingRepo.AddBooking(booking);

            
            
            // Razorpay work start

            string _RazorKey = _razorpaySettingsService.GetRazorpayKey();
            string _RazorSecret = _razorpaySettingsService.GetRazorpaySecret();

            RazorpayClient client = new RazorpayClient(_RazorKey, _RazorSecret);

            var options = new Dictionary<string, object>
            {
                { "amount", booking.TotalAmount * 100 }, 
                { "currency", "INR" },
                { "receipt", booking.Id.ToString() },
                { "payment_capture", 1 }
            };
            var order = client.Order.Create(options);

            booking.PaymentReference = order["id"].ToString();
            _bookingRepo.UpdateBooking(booking);

            return Json(new { orderId = booking.PaymentReference, key = _RazorKey, amount = booking.TotalAmount * 100, bookingId = booking.Id });

            // Razorpay over


            ////task1
            //var bookingUser = _userRepo.GetUser(currentUserId);
            //if(bookingUser.WalletBalance < booking.TotalAmount)
            //{
            //    ModelState.AddModelError("", "Insufficient wallet balance.");
            //    return View(model);
            //}
            //bookingUser.WalletBalance -= booking.TotalAmount;

            //        //task2
            //var hostUser = _userRepo.GetUser(charger.UserId);
            //hostUser.WalletBalance += booking.HostEarnings;
            //        //task3
            //charger.ChargerIncome += (int)booking.HostEarnings;
            //        //task4
            //var adminUser = _userRepo.GetAllUsers().FirstOrDefault(u => u.IsAdmin);

            //if (adminUser != null)
            //{
            //    adminUser.WalletBalance += booking.CommissionAmount;
            //    _userRepo.UpdateUser(adminUser);
            //}


            //// updating
            //_userRepo.UpdateUser(bookingUser);
            //_userRepo.UpdateUser(hostUser);
            //_chargerRepo.UpdateCharger(charger);


            ////return RedirectToAction("Details", new { id = booking.Id });
            ////return RedirectToAction("Index");
            //return RedirectToAction("UsersBookingIndex",  new { id = currentUserId });
        }




        public IActionResult Details(int id)
        {
            var booking = _bookingRepo.GetBooking(id);
            if (booking == null)
            {
                Response.StatusCode = 404;
                return View("ChargerNotFound", id);
            }

            var viewModel = new BookingDetailsViewModel
            {
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                TotalAmount = booking.TotalAmount,
                SizeType = booking.SizeType.ToString(),
                BatterySize = booking.BatterySize,
                ExpectedBatteryPercentage = booking.ExpectedBatteryPercentage,
                Status = booking.Status.ToString(),
                UserEmail = booking.User?.Email,
                HostEmail = booking.Charger?.User?.Email
            };
            return View(viewModel);
        }



        [HttpGet]
        public IActionResult Delete(int id)
        {
            var booking = _bookingRepo.GetBooking(id);
            if (booking == null)
            {
                Response.StatusCode = 404;
                return View("BookingNotFound", id);
            }
            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var booking = _bookingRepo.GetBooking(id);
            if (booking == null)
            {
                Response.StatusCode = 404;
                return View("BookingNotFound", id);
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }
            int currentUserId = int.Parse(userIdClaim);




            var bookingUser = _userRepo.GetUser(currentUserId);
            //if (bookingUser.WalletBalance < booking.TotalAmount)
            //{
            //    ModelState.AddModelError("", "Insufficient wallet balance.");
            //    return View(booking);
            //}
            //bookingUser.WalletBalance += booking.TotalAmount;
            bookingUser.WalletBalance = bookingUser.WalletBalance + booking.TotalAmount;





            var charger = _chargerRepo.GetCharger(booking.ChargerId);
            if (charger == null)
            {
                ModelState.AddModelError("", "Associated charger not found.");
                return View(booking);
            }
            var hostUser = _userRepo.GetUser(charger.UserId);
            hostUser.WalletBalance -= (booking.HostEarnings );





            charger.ChargerIncome -= (int)booking.HostEarnings;
            //task4
            var adminUser = _userRepo.GetAllUsers().FirstOrDefault(u => u.IsAdmin);

            if (adminUser != null)
            {
                adminUser.WalletBalance -= booking.CommissionAmount;
                _userRepo.UpdateUser(adminUser);
            }


            //_bookingRepo.DeleteBooking(id);
            //not deleteing, just mark cancelled still makes payments
            var currBooking = _bookingRepo.GetBooking(id);
            currBooking.Status = BookingStatus.Cancelled;
            _bookingRepo.UpdateBooking(currBooking);


            _userRepo.UpdateUser(bookingUser);
            _userRepo.UpdateUser(hostUser);
            _chargerRepo.UpdateCharger(charger);


            return RedirectToAction("UsersBookingIndex", new { id = currentUserId });
        }


        [HttpPost]
        public IActionResult VerifyPayment([FromBody] RazorpayResponse response)
        {
            try
            {
                // Razorpay work start

                var booking = _bookingRepo.GetBooking(response.bookingId);
                if (booking == null) return NotFound();

                var attributes = new Dictionary<string, string>
                {
                    { "razorpay_payment_id", response.razorpay_payment_id },
                    { "razorpay_order_id", response.razorpay_order_id },
                    { "razorpay_signature", response.razorpay_signature }
                };

                Utils.verifyPaymentSignature(attributes);

                //Razorpay work over


                booking.Status = BookingStatus.Confirmed;
                _bookingRepo.UpdateBooking(booking);

                var bookingUser = _userRepo.GetUser(booking.UserId);
                var charger = _chargerRepo.GetCharger(booking.ChargerId);
                var hostUser = _userRepo.GetUser(charger.UserId);
                var adminUser = _userRepo.GetAllUsers().FirstOrDefault(u => u.IsAdmin);


                //here it is not require b'coz it is done by razorpay payment
                //bookingUser.WalletBalance -= booking.TotalAmount;
                hostUser.WalletBalance += booking.HostEarnings;
                charger.ChargerIncome += (int)booking.HostEarnings;
                if (adminUser != null)
                {
                    adminUser.WalletBalance += booking.CommissionAmount;
                    adminUser.TurnOver += booking.TotalAmount;
                    _userRepo.UpdateUser(adminUser);
                }

                //_userRepo.UpdateUser(bookingUser);
                _userRepo.UpdateUser(hostUser);
                _chargerRepo.UpdateCharger(charger);


                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized();
                }

                int currentUserId = int.Parse(userIdClaim);

                return RedirectToAction("UsersBookingIndex",  new { id = currentUserId });
                //return Ok(new { status = "success", redirectUrl = Url.Action("UsersBookingIndex", new { id = currentUserId }) });

            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "failed", message = ex.Message });
            }

            //return Ok(new { message = "Payment verified successfully." });
        }






    }
}
