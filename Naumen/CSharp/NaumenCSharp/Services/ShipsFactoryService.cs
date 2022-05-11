using System;
using NaumenCSharp.Models;

namespace NaumenCSharp.Services
{
    public class ShipsFactoryService
    {
        private ShipsFactory _shipsFactory;

        public void SetupNewFactory(int size)
        {
            _shipsFactory = new ShipsFactory(size);
        }

        public void AddShipInQueue(Ship ship)
        {
            CheckFactorySetup();
            _shipsFactory.AddShipInQueue(ship);
        }

        private void CheckFactorySetup()
        {
            if (_shipsFactory == null)
                throw new ArgumentNullException(nameof(_shipsFactory), "Factory should be setup before use.");
        }

        public int? GetNextRepairStartTime()
        {
            return _shipsFactory?.GetNextShipRepairStart();
        }
    }
}