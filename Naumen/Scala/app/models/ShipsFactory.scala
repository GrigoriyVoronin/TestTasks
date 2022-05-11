package models

class ShipsFactory (maxSize: Int) {
  private val _maxSize: Int = maxSize
  private var _alreadyRepairedPointer: ShipRepairNode  = null
  private var _currentShipPointer: ShipRepairNode  = null
  private var _repairEndTime: Int = 0
  private var _head : ShipRepairNode = null
  private var _tail : ShipRepairNode = null


  def addShipInQueue(ship: Ship): Unit = {
    val currentTime = ship.timeOfArrival
    val currentRepairingShipsCount = calculateShipsInRepairing(currentTime)
    if (currentRepairingShipsCount == _maxSize) {
      addShipInEnd(ship, -1, -1)
    }
    else {
      val repairStartTime = Math.max(currentTime, _repairEndTime);
      _repairEndTime = repairStartTime + ship.handleTime
      addShipInEnd(ship, repairStartTime, _repairEndTime)
    }
  }

  def nextRepairStartTime(): RepairTime = {
    if (_currentShipPointer == null) {
      _currentShipPointer = _head
      if (_currentShipPointer == null){
        return null
      }
      return new RepairTime(_currentShipPointer.repairStartTime)
    }

    if (_currentShipPointer.nextNode == null)
      return null

    _currentShipPointer = _currentShipPointer.nextNode
    new RepairTime(_currentShipPointer.repairStartTime)
  }


  private def calculateShipsInRepairing(currentTime: Int):Int = {
     if (_alreadyRepairedPointer == null)
       _alreadyRepairedPointer = _head

    var currentNode = _alreadyRepairedPointer
    var shipsOnRepairingCount = 0
    while (currentNode != null) {
      if (currentNode.repairEndTime > currentTime) {
        shipsOnRepairingCount +=1
      }
      else {
        _alreadyRepairedPointer = currentNode
      }

      currentNode  = currentNode.nextNode
    }

    shipsOnRepairingCount
  }

  private def addShipInEnd(ship: Ship, repairStartTime: Int, repairEndTime: Int): Unit = {
    val node = new ShipRepairNode(ship, repairStartTime, repairEndTime)
    if (_tail == null){
      _tail = node
      _head = node
    }
    else {
      _tail.nextNode = node
      _tail = node
    }
  }
}
