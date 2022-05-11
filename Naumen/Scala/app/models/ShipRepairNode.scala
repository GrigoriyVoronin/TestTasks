package models

class ShipRepairNode (val ship: Ship, val repairStartTime: Int, val repairEndTime: Int){
  var nextNode: ShipRepairNode = null
}
