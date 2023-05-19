namespace Exam_3;

using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Exam_3.Models;

public class InMemoryDatabase
{
    public List<User> Users { get; private set; } = new List<User>();
    public List<Car> Cars { get; private set; } = new List<Car>();
    public List<Visit> Visits { get; private set; } = new List<Visit>();

    public void Create()
    {
        SetStaticUserData();
        SetStaticCarData();
        RelateUsersAndCars();
        SetStaticVisitData();
    }

    private List<User> SetStaticUserData()
    {
        if (Users.Count() != 0)
            return Users;

        int count = 10;
        var users = new List<User>();
        users.Add(CreateUser(1, true));
        users.Add(CreateUser(2, true));
        users.Add(CreateUser(3, true));
        for (int i = 4; i <= count; i++)
        {
            users.Add(CreateUser(i));
        }

        Users = users;
        return users;
    }

    private User CreateUser(int i, bool isMechanic = false)
    {
        var faker = new Faker("en");
        return new User() { Id = i + 1, Username = faker.Internet.UserName(), Password = faker.Internet.Password(), IsMechanic = isMechanic };
    }

    private List<Car> SetStaticCarData()
    {
        int count = 10;
        var cars = new List<Car>();
        cars.Add(CreateCar(1));
        cars.Add(CreateCar(2));
        cars.Add(CreateCar(3));
        for (int i = 4; i <= count; i++)
        {
            cars.Add(CreateCar(i));
        }

        Cars = cars;
        return cars;
    }

    private Car CreateCar(int i)
    {
        var faker = new Faker("en");
        var car = new Car()
        {
            Id = i + 1,
            Model = faker.Vehicle.Model()
        };
        return car;
    }

    private void RelateUsersAndCars()
    {
        Faker faker = new Faker("en");
        for (int i = 0; i < Users.Count(); i++)
        {
            if (Users[i].IsMechanic)
                continue;
            Users[i].Cars = faker.PickRandom<Car>(Cars, faker.Random.Int(1, 2)).ToList();
        }

        for (int i = 0; i < Cars.Count(); i++)
        {
            Cars[i].MechanicId = faker.PickRandom<User>(Users.Where(u => u.IsMechanic)).Id;
        }
    }

    private void SetStaticVisitData()
    {
        Faker faker = new Faker("en");
        int j = 1;
        for (int i = 0; i < Cars.Count(); i++)
        {
            Visit visit = CreateVisit(Cars[i].Id, j);
            Visits.Add(visit);
            Cars[i].VisitIds = new List<int> { visit.Id };
            j++;
        }
    }

    private Visit CreateVisit(int carId, int index)
    {
        long ticks;
        if (Visits.Count() == 0)
            ticks = DateTime.UtcNow.Ticks;
        else
        {
            var t = Visits.Last().Ticks;
            ticks = (new DateTime(Visits.Last().Ticks)).AddHours(1).Ticks;
        }

        return new Visit() { Id = index, CarId = carId, Ticks = ticks };
    }
}
