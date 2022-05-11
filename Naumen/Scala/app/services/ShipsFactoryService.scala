package services

import javax.inject._
import models._

@Singleton
class ShipsFactoryService @Inject() () {
  private var _shipsFactory: ShipsFactory = null

  def setupNewFactory (size: Int): Unit = {
    _shipsFactory = new ShipsFactory(size)
  }

  def addShipInQueue(ship: Ship): Unit = {
    if (_shipsFactory == null) {
      throw new NullPointerException("Factory should be setup before use.")
    }
    _shipsFactory.addShipInQueue(ship)
  }

    def nextRepairStartTime(): RepairTime = {
      if (_shipsFactory == null)
        null
      else
        _shipsFactory.nextRepairStartTime()
    }
}