#region using

using System;
using System.Reflection;

#endregion

namespace Outsourcing
{
    [Route("apps/cars-api/v1/cars")]
    public class CarsController : IController
    {
        private readonly ICarService carService;
        private readonly ILog logger;

        public CarsController(ICarService carService, ILog logger)
        {
            this.carService = carService;
            this.logger = logger;
        }

        [HttpPost]
        public ActionResult<Car> Create(Car car)
        {
            try
            {
                if (!carService.IsValidCar(car, out var errorMessage))
                    return ActionResult.PreconditionFailed.WithMessage(errorMessage).Empty<Car>();
                if (carService.IsCarExist(car.Id, out _))
                    return ActionResult.Conflict.WithMessage("Car already exist").Empty<Car>();

                car = carService.Add(car);
                return ActionResult.Created.WithContent(car);
            }
            catch (Exception exception)
            {
                return LogAndReturnError(exception);
            }
        }

        [Route("{id}")]
        [HttpPut]
        public ActionResult<Car> Update(Guid id, Car updateCar)
        {
            try
            {
                if (id != updateCar.Id)
                    return ActionResult.BadRequest.Empty<Car>();
                if (!carService.IsCarExist(id, out _))
                    return ActionResult.NotFound.Empty<Car>();
                if (!carService.IsValidCar(updateCar, out var errorMessage))
                    return ActionResult.PreconditionFailed.WithMessage(errorMessage).Empty<Car>();

                updateCar = carService.Update(updateCar);
                return ActionResult.Ok.WithContent(updateCar);
            }
            catch (Exception exception)
            {
                return LogAndReturnError(exception);
            }
        }

        [Route("{id}")]
        [HttpPatch]
        public ActionResult<Car> Patch(Guid id, Car patchCar)
        {
            try
            {
                if (!carService.IsCarExist(id, out _))
                    return ActionResult.NotFound.Empty<Car>();
                if (!carService.IsValidCar(patchCar, out var errorMessage))
                    return ActionResult.PreconditionFailed.WithMessage(errorMessage).Empty<Car>();

                patchCar = carService.Patch(patchCar);
                return ActionResult.Ok.WithContent(patchCar);
            }
            catch (Exception exception)
            {
                return LogAndReturnError(exception);
            }
        }

        [Route("{id}")]
        [HttpGet]
        public ActionResult<Car> Get(Guid id)
        {
            try
            {
                return carService.IsCarExist(id, out var car)
                    ? ActionResult.Ok.WithContent(car)
                    : ActionResult.NotFound.Empty<Car>();
            }
            catch (Exception exception)
            {
                return LogAndReturnError(exception);
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            try
            {
                if (!carService.IsCarExist(id, out _))
                    return ActionResult.NotFound.Empty<Car>();

                carService.Delete(id);
                return ActionResult.Ok;
            }
            catch (Exception exception)
            {
                return LogAndReturnError(exception);
            }
        }

        private ActionResult<Car> LogAndReturnError(Exception exception)
        {
            logger.Log(exception.Message);
            return ActionResult.InternalServerError.Empty<Car>();
        }
    }

    public interface ICarsRepository
    {
        Car Add(Car model);

        Car Update(Car model);

        Car Find(Guid id);

        void Delete(Guid id);
    }

    public interface ILog
    {
        void Log(string error);
    }

    public interface ICarService
    {
        Car Add(Car car);
        Car Update(Car car);
        Car Patch(Car carPatchModel);
        void Delete(Guid id);
        bool IsValidCar(Car car, out string errorMessage);
        bool IsCarExist(Guid id, out Car car);
    }

    public class CarService : ICarService
    {
        private readonly ICarsRepository carsRepository;

        public CarService(ICarsRepository carsRepository)
        {
            this.carsRepository = carsRepository;
        }

        public Car Add(Car car)
        {
            return carsRepository.Add(car);
        }

        Car ICarService.Update(Car car)
        {
            return carsRepository.Update(car);
        }

        public Car Patch(Car carPatchModel)
        {
            var updateCarModel = new Car();
            var carType = typeof(Car);
            var carProperties = carType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance |
                               BindingFlags.GetProperty | BindingFlags.SetProperty);
            foreach (var property in carProperties)
            {
                var patchValue = property.GetValue(carPatchModel);
                if (patchValue != null)
                    property.SetValue(updateCarModel, patchValue);
            }

            return carsRepository.Update(updateCarModel);
        }

        public void Delete(Guid id)
        {
            carsRepository.Delete(id);
        }

        public bool IsCarExist(Guid id, out Car car)
        {
            car = carsRepository.Find(id);
            return car != null;
        }

        public bool IsValidCar(Car car, out string errorMessage)
        {
            errorMessage = null;
            if (car.Weight <= 1000 && car.Weight >= 0)
                return true;

            errorMessage = "Wrong format of weight parameter";
            return false;
        }
    }

    public class Car
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public string Mark { get; set; }
        public string Model { get; set; }
    }


    public sealed class ActionResult<T> : ActionResult
        where T : class
    {
        public ActionResult(StatusCode statusCode, T content)
            : base(statusCode)
        {
            Content = content;
        }

        public T Content { get; }
    }

    public class ActionResult : IActionResult
    {
        protected ActionResult(StatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public string Message { get; private set; }

        public static ActionResult Ok => new ActionResult(StatusCode.Ok);
        public static ActionResult NotFound => new ActionResult(StatusCode.NotFound);
        public static ActionResult Created => new ActionResult(StatusCode.Created);
        public static ActionResult BadRequest => new ActionResult(StatusCode.BadRequest);
        public static ActionResult Conflict => new ActionResult(StatusCode.Conflict);
        public static ActionResult PreconditionFailed => new ActionResult(StatusCode.PreconditionFailed);
        public static ActionResult InternalServerError => new ActionResult(StatusCode.InternalServerError);

        public StatusCode StatusCode { get; }

        public ActionResult<T> Empty<T>()
            where T : class
        {
            return new ActionResult<T>(StatusCode, null);
        }

        public ActionResult WithMessage(string message)
        {
            Message = message;
            return this;
        }

        public ActionResult<T> WithContent<T>(T content)
            where T : class
        {
            return new ActionResult<T>(StatusCode, content);
        }
    }

    public interface IActionResult
    {
        public StatusCode StatusCode { get; }
    }

    public enum StatusCode
    {
        Ok = 200,
        Created = 201,
        BadRequest = 400,
        NotFound = 404,
        Conflict = 409,
        PreconditionFailed = 412,
        InternalServerError = 500
    }

    internal interface IController
    {
    }
}