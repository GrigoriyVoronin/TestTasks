package controllers

import javax.inject._
import play.api.mvc._
import services._
import models._
import play.api.Logger


@Singleton
class ShipController @Inject()(cc : ControllerComponents, shipsFactoryService : ShipsFactoryService) extends AbstractController(cc) {

  def setupFactorySize() = Action { request: Request[AnyContent] =>
    try {
      val jsonBody = request.body.asJson
      val numberOfPlaces = jsonBody.get("numberOfPlaces").as[String].toInt
      shipsFactoryService.setupNewFactory(numberOfPlaces)
      Ok("")
    }
    catch {
      case _: Throwable => BadRequest("Bad Request")
    }
  }

  def addShip() = Action { request: Request[AnyContent] =>
    try {
      val jsonBody = request.body.asJson
      val timeOfArrival = jsonBody.get("timeOfArrival").as[String].toInt
      val handleTime = jsonBody.get("handleTime").as[String].toInt
      val ship = new Ship(timeOfArrival, handleTime)
      shipsFactoryService.addShipInQueue(ship)
      Ok("")
    }
    catch {
      case _: Throwable => BadRequest("Bad Request")
    }
  }

  def nextShipRepairTime() = Action { request: Request[AnyContent] =>

      val startTime = shipsFactoryService.nextRepairStartTime()
      if (startTime == null){
        Ok("")
      }
      else {
        Ok("{response: \"" + startTime.repairStartTime + "\"}")
      }
  }

  def methodNotFound(str: String): Action[AnyContent] = Action { request: Request[AnyContent] =>
    NotFound
  }
}