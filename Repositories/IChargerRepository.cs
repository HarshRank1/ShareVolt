using ShareVolt.Models;
using System.Collections.Generic;

namespace ShareVolt.Repositories
{
    public interface IChargerRepository
    {
        Charger GetCharger(int id);
        IEnumerable<Charger> GetAllChargers();
        Charger AddCharger(Charger charger);
        Charger UpdateCharger(Charger charger);
        Charger DeleteCharger(int id);
        int GetChargerIncome(int id);
        IEnumerable<Charger> GetChargersByUserId(int id);
    }
}
