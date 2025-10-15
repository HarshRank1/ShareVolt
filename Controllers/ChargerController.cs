using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using ShareVolt.Models;
using ShareVolt.Repositories;
using System;
using System.Linq;
using System.Security.Claims;

namespace ShareVolt.Controllers
{
    
    public class ChargerController : Controller
    {
        private readonly IChargerRepository _chargerRepo;

        public ChargerController(IChargerRepository chargerRepo)
        {
            _chargerRepo = chargerRepo;
        }

        //public IActionResult Index()
        //{
        //    var model = _chargerRepo.GetAllChargers();
        //    return View(model);
        //}
        // In ShareVolt/Controllers/ChargerController.cs

        public IActionResult Index(string searchDistrict, string searchState, string searchCountry, bool? isAvailable, string searchChargerType, int page = 1)
        {
            int pageSize = 6;
            ViewData["CurrentDistrict"] = searchDistrict;
            ViewData["CurrentState"] = searchState;
            ViewData["CurrentCountry"] = searchCountry;
            ViewData["CurrentAvailability"] = isAvailable;
            ViewData["CurrentChargerType"] = searchChargerType;
            ViewData["CurrentPage"] = page;

            var chargers = _chargerRepo.GetAllChargers().AsQueryable();

            
            if (!String.IsNullOrEmpty(searchDistrict))
            {
                chargers = chargers.Where(c => c.District != null && c.District.Contains(searchDistrict, StringComparison.OrdinalIgnoreCase));
            }

            if (!String.IsNullOrEmpty(searchState))
            {
                chargers = chargers.Where(c => c.State != null && c.State.Contains(searchState, StringComparison.OrdinalIgnoreCase));
            }

            if (!String.IsNullOrEmpty(searchCountry))
            {
                chargers = chargers.Where(c => c.Country != null && c.Country.Contains(searchCountry, StringComparison.OrdinalIgnoreCase));
            }

            if (isAvailable.HasValue)
            {
                chargers = chargers.Where(c => c.IsAvailable == isAvailable.Value);
            }

            if (!string.IsNullOrEmpty(searchChargerType) && Enum.TryParse<ChargerType>(searchChargerType, out var parsedType))
            {
                chargers = chargers.Where(c => c.Type == parsedType);
            }

            int totalCount = chargers.Count();
            var pagedChargers = chargers
                        .OrderBy(c => c.Id)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            ViewData["TotalPages"] = (int)Math.Ceiling(totalCount / (double)pageSize);

            return View(pagedChargers);
            //return View(chargers.ToList());
        }

        [Authorize]
        public IActionResult UsersChargersIndex(int id)
        {
            var model = _chargerRepo.GetChargersByUserId(id);
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult HostChargers()
        {
            var model = _chargerRepo.GetAllChargers();
            return View(model);
        }

        [Authorize] 
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ChargerTypes = Enum.GetValues(typeof(ChargerType));
            ViewBag.ChargerOptions = ChargerConfig.ChargerOptions;

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(Charger model)
        {
            if (ModelState.IsValid)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized();
                }

                int currentUserId = int.Parse(userIdClaim);

                Charger newCharger = new Charger
                {
                    Location = model.Location,
                    District = model.District,
                    State = model.State,
                    Country = model.Country,
                    GoogleMapsUrl = model.GoogleMapsUrl,
                    Type = model.Type,
                    PricePerKWh = model.PricePerKWh,
                    IsAvailable = true,
                    ChargerIncome = 0,
                    ChargerSpeed = model.ChargerSpeed,
                    UserId = currentUserId

                };

                _chargerRepo.AddCharger(newCharger);
                return RedirectToAction("Details", new { id = newCharger.Id });
            }

            ViewBag.ChargerTypes = Enum.GetValues(typeof(ChargerType));
            ViewBag.ChargerOptions = ChargerConfig.ChargerOptions;
            return View(model);
        }


        public IActionResult Details(int id)
        {
            var charger = _chargerRepo.GetCharger(id);
            if (charger == null)
            {
                Response.StatusCode = 404;
                return View("ChargerNotFound", id);
            }
            return View(charger);
        }


        [Authorize]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var charger = _chargerRepo.GetCharger(id);
            if (charger == null)
            {
                return NotFound();
            }

            ViewBag.ChargerTypes = Enum.GetValues(typeof(ChargerType));
            ViewBag.ChargerOptions = ChargerConfig.ChargerOptions;
            return View(charger);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(Charger model)
        {
            if (ModelState.IsValid)
            {
                var charger = _chargerRepo.GetCharger(model.Id);
                if (charger == null)
                {
                    return NotFound();
                }

                charger.Location = model.Location;
                charger.District = model.District;
                charger.State = model.State;
                charger.Country = model.Country;
                charger.GoogleMapsUrl = model.GoogleMapsUrl;
                charger.Type = model.Type;
                charger.PricePerKWh = model.PricePerKWh;
                charger.IsAvailable = model.IsAvailable;
                charger.ChargerSpeed = model.ChargerSpeed;

                _chargerRepo.UpdateCharger(charger);
                return RedirectToAction("Index");
            }

            ViewBag.ChargerTypes = Enum.GetValues(typeof(ChargerType));
            ViewBag.ChargerOptions = ChargerConfig.ChargerOptions;
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var charger = _chargerRepo.GetCharger(id);
            if (charger == null)
            {
                return NotFound();
            }
            return View(charger);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _chargerRepo.DeleteCharger(id);
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetChargerMeta(ChargerType type)
        {
            var meta = ChargerConfig.ChargerOptions.FirstOrDefault(c => c.Type == type);
            if (meta == null) return NotFound();

            return Json(meta);
        }





    }
}
