
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Ddd.Infrastructure;

namespace Ddd.Taxi.Domain
{
    public class DriversRepository
    {
        public void FillDriverToOrder(int driverId, TaxiOrder order)
        {
            if (driverId == 15)
            {
                var car = new Car("Baklazhan", "Lada sedan", "A123BT 66");
                order.SetDriver(new Driver(driverId, new PersonName("Drive", "Driverson"), car));
                order.SetCar(order.Driver.Car);
            }
            else
				throw new Exception("Unknown driver id " + driverId);
		}
    }

    public class TaxiApi : ITaxiApi<TaxiOrder>
	{
		private readonly DriversRepository driversRepo;
		private readonly Func<DateTime> currentTime;
		private int idCounter;

		public TaxiApi(DriversRepository driversRepo, Func<DateTime> currentTime)
		{
			this.driversRepo = driversRepo;
			this.currentTime = currentTime;
		}

		public TaxiOrder CreateOrderWithoutDestination(string firstName, string lastName, string street, string building)
		{
			return new TaxiOrder(idCounter++,
				new PersonName(firstName, lastName),
				new Address(street, building),
				currentTime);
		}

		public void UpdateDestination(TaxiOrder order, string street, string building)
		{
			order.UpdateDestination(new Address(street, building));
		}

		public void AssignDriver(TaxiOrder order, int driverId)
		{
			order.AssignDriver(driverId, driversRepo, currentTime);
		}

		public void UnassignDriver(TaxiOrder order)
		{
			order.UnassignDriver();
		}

		public string GetDriverFullInfo(TaxiOrder order)
		{
			return order.GetDriverFullInfo();
		}

		public string GetShortOrderInfo(TaxiOrder order)
		{
			return order.GetShortOrderInfo();
		}

		public void Cancel(TaxiOrder order)
		{
			order.Cancel(currentTime);
		}

		public void StartRide(TaxiOrder order)
		{
			order.StartRide(currentTime);
		}

		public void FinishRide(TaxiOrder order)
		{
			order.FinishRide(currentTime);
		}
	}

	public class TaxiOrder : Entity<int>
	{
		public PersonName ClientName { get; private set; }
		public Address Start { get; private set; } 
		public Address Destination { get; private set; }
		public Driver Driver { get; private set; }
		public Car Car { get; private set; } 
		public TaxiOrderStatus Status { get; private set; } 
		public DateTimeInfo TimeInformation { get; private set; }


		public TaxiOrder(int id,
			PersonName client= null,
			Address startAddress = null,
			Func<DateTime> currentTime = null
			) : base(id)
		{
			ClientName = client;
			Start = startAddress;
			TimeInformation = new DateTimeInfo() { CreationTime = currentTime() };
			Destination = new Address(null, null);
			Driver =new Driver(0, new PersonName(null, null), new Car(null, null, null));
		}
		
		public void SetDriver(Driver driver) => Driver = driver;

		public void SetCar(Car car) => Car = car;

        public void UpdateDestination(Address destinationAddress)=>
            Destination = destinationAddress;

        public void AssignDriver(int driverId, DriversRepository driversRepo, Func<DateTime> currentTime)
		{
            if (Status == TaxiOrderStatus.InProgress || Status == TaxiOrderStatus.WaitingCarArrival)
                throw new InvalidOperationException("Invalid operation becouse current status is " +Status.ToString());

            driversRepo.FillDriverToOrder(driverId, this);
			TimeInformation.DriverAssignmentTime = currentTime();
			Status = TaxiOrderStatus.WaitingCarArrival;
		}

		public void UnassignDriver()
		{
			if (Status == TaxiOrderStatus.InProgress || Status == TaxiOrderStatus.WaitingForDriver || Driver.Id == 0)
				throw new InvalidOperationException("Invalid operation becouse current status is " +Status.ToString());

			Driver = new Driver(0, new PersonName(null, null), new Car(null, null, null));
			Car = new Car(null, null, null);
			Status = TaxiOrderStatus.WaitingForDriver;
		}

		public string GetDriverFullInfo()
		{
			return string.Join(" ",
				"Id: " + Driver.Id,
				"DriverName: " + FormatName(Driver.DriverName.FirstName, Driver.DriverName.LastName),
				"Color: " + Car.Color,
				"CarModel: " + Car.Model,
				"PlateNumber: " + Car.PlateNumber);
		}

		public string GetShortOrderInfo()
		{
			return string.Join(" ",
				"OrderId: " + Id,
				"Status: " + Status,
				"Client: " + FormatName(ClientName.FirstName, ClientName.LastName),
				"Driver: " + FormatName(Driver.DriverName.FirstName, Driver.DriverName.LastName),
				"From: " + FormatAddress(Start.Street, Start.Building),
				"To: " + FormatAddress(Destination.Street, Destination.Building),
				"LastProgressTime: " + GetLastProgressTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
		}

		private DateTime GetLastProgressTime()
		{
			if (Status == TaxiOrderStatus.WaitingForDriver) return TimeInformation.CreationTime;
			if (Status == TaxiOrderStatus.WaitingCarArrival) return TimeInformation.DriverAssignmentTime;
			if (Status == TaxiOrderStatus.InProgress) return TimeInformation.StartRideTime;
			if (Status == TaxiOrderStatus.Finished) return TimeInformation.FinishRideTime;
			if (Status == TaxiOrderStatus.Canceled) return TimeInformation.CancelTime;
			throw new Exception();
		}

		private string FormatName(string firstName, string lastName)
		{
			return string.Join(" ", new[] { firstName, lastName }.Where(n => n != null));
		}

		private string FormatAddress(string street, string building)
		{
			return string.Join(" ", new[] { street, building }.Where(n => n != null));
		}

		public void Cancel(Func<DateTime> currentTime)
		{
			if (Status == TaxiOrderStatus.InProgress)
				throw new InvalidOperationException("Invalid operation becouse current status is " +Status.ToString());

			Status = TaxiOrderStatus.Canceled;
			TimeInformation.CancelTime = currentTime();
		}

		public void StartRide(Func<DateTime> currentTime)
		{
			if (Status == TaxiOrderStatus.WaitingForDriver || Driver.Id == 0)
				throw new InvalidOperationException("Invalid operation becouse current status is " +Status.ToString());

			Status = TaxiOrderStatus.InProgress;
			TimeInformation.StartRideTime = currentTime();
		}

		public void FinishRide(Func<DateTime> currentTime)
		{
			if (Status != TaxiOrderStatus.InProgress)
				throw new InvalidOperationException("Invalid operation becouse current status is " + Status.ToString());

			Status = TaxiOrderStatus.Finished;
			TimeInformation.FinishRideTime = currentTime();
		}
	}

	public class Driver : Entity<int>
	{
		public Driver(int id, PersonName driverName, Car car) : base(id)
		{
			DriverName = driverName;
			Car = car;
		}

		public PersonName DriverName { get; }
		public Car Car { get; }
	}

	public class Car : ValueType<Car>
	{
		public string Color { get; }
		public string Model { get; }
		public string PlateNumber { get; }
		public Car(string color, string model, string plateNumber)
		{
			Color = color;
			Model = model;
			PlateNumber = plateNumber;
		}
	}

    public class DateTimeInfo
    {
        public DateTime CreationTime { get; set; }
        public DateTime DriverAssignmentTime { get; set; }
        public DateTime CancelTime { get; set; }
        public DateTime StartRideTime { get; set; }
        public DateTime FinishRideTime { get; set; }
    }
}
