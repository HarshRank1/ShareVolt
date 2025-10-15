using Microsoft.EntityFrameworkCore;
using ShareVolt.Models;
using System.Collections.Generic;
using System.Linq;

namespace ShareVolt.Repositories
{
    public class SQLChargerRepository : IChargerRepository
    {
        private readonly ShareVoltDbContext _context;

        public SQLChargerRepository(ShareVoltDbContext context)
        {
            _context = context;
        }

        public Charger AddCharger(Charger charger)
        {
            _context.Chargers.Add(charger);
            _context.SaveChanges();
            return charger;
        }

        public Charger DeleteCharger(int id)
        {
            var charger = _context.Chargers.Find(id);
            if (charger != null)
            {
                _context.Chargers.Remove(charger);
                _context.SaveChanges();
            }
            return charger;
        }

        public IEnumerable<Charger> GetAllChargers()
        {
            return _context.Chargers
                .Include(c => c.User)
                .Include(c => c.Bookings)
                .ToList();
        }

        public Charger GetCharger(int id)
        {
            return _context.Chargers
                .Include(c => c.User)
                .Include(c => c.Bookings)
                .FirstOrDefault(c => c.Id == id);
        }

        public Charger UpdateCharger(Charger charger)
        {
            _context.Chargers.Update(charger);
            _context.SaveChanges();
            return charger;
        }

        public int GetChargerIncome(int id)
        {
            var charger = _context.Chargers.FirstOrDefault(c => c.Id == id);
            if(charger == null)
            {
                return 0;
            }
            return charger.ChargerIncome;
        }

        public IEnumerable<Charger> GetChargersByUserId(int id)
        {
            return _context.Chargers
               .Include(c => c.User)
               .Include(c => c.Bookings)
               .Where(c => c.UserId == id)
               .ToList();
            throw new System.NotImplementedException();
        }
    }
}
