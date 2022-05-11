using System;
using System.Collections.Generic;

namespace NaumenCSharp.Models
{
    public class ShipsFactory
    {
        private readonly int _maxSize;
        private readonly LinkedList<ShipRepairNode> _shipsInFactory;
        private LinkedListNode<ShipRepairNode> _alreadyRepairedHead;
        private LinkedListNode<ShipRepairNode> _head;
        private int _repairEndTime;

        public ShipsFactory(int maxSize)
        {
            _maxSize = maxSize;
            _shipsInFactory = new LinkedList<ShipRepairNode>();
        }

        public void AddShipInQueue(Ship ship)
        {
            var currentTime = ship.TimeOfArrival;
            var currentRepairingShipsCount = CalculateShipInRepairing(currentTime);
            if (currentRepairingShipsCount == _maxSize)
            {
                _shipsInFactory.AddLast(new ShipRepairNode(ship, -1, -1));
            }
            else
            {
                var repairStartTime = Math.Max(currentTime, _repairEndTime);
                _repairEndTime = repairStartTime + ship.HandleTime;
                _shipsInFactory.AddLast(new ShipRepairNode(ship, repairStartTime, _repairEndTime));
            }
        }

        private int CalculateShipInRepairing(int currentTime)
        {
            _alreadyRepairedHead ??= _shipsInFactory.First;
            var currentNode = _alreadyRepairedHead;
            var shipsOnRepairingCount = 0;
            while (currentNode != null)
            {
                if (currentNode.Value.RepairEndTime > currentTime)
                    shipsOnRepairingCount++;
                else
                    _alreadyRepairedHead = currentNode;

                currentNode = currentNode.Next;
            }

            return shipsOnRepairingCount;
        }

        public int? GetNextShipRepairStart()
        {
            if (_head == null)
            {
                _head = _shipsInFactory.First;
                return _head?.Value.RepairStartTime;
            }

            if (_head.Next == null)
                return null;

            _head = _head.Next;
            return _head.Value.RepairStartTime;
        }
    }
}