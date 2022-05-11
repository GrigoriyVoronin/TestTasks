using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NaumenCSharp.Models;
using NaumenCSharp.Services;

namespace NaumenCSharp.Controllers
{
    [Route("")]
    [ApiController]
    public class ShipController : ControllerBase
    {
        private readonly ShipsFactoryService _factoryService;

        public ShipController(ShipsFactoryService factoryService)
        {
            _factoryService = factoryService;
        }

        [HttpPost("numberOfPlaces")]
        public ActionResult SetupFactorySize([Required] [FromBody] SetupNumberOfPlacesRequest request)
        {
            try
            {
                _factoryService.SetupNewFactory(request.NumberOfPlaces);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("ship")]
        public ActionResult AddShip([Required] [FromBody] Ship ship)
        {
            try
            {
                _factoryService.AddShipInQueue(ship);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet("next")]
        public ActionResult GetNextShipRepairTime()
        {
            try
            {
                var repairEndTime = _factoryService.GetNextRepairStartTime();
                if (repairEndTime == null)
                    return Ok();

                return Ok(new GetNextShipResponse {Response = (int) repairEndTime});
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}