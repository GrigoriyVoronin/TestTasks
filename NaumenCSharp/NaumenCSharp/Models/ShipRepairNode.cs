namespace NaumenCSharp.Models
{
    public class ShipRepairNode
    {
        public ShipRepairNode(Ship ship, int repairStartTime, int repairEndTime)
        {
            Ship = ship;
            RepairStartTime = repairStartTime;
            RepairEndTime = repairEndTime;
        }

        public Ship Ship { get; }
        public int RepairStartTime { get; }
        public int RepairEndTime { get; }
    }
}