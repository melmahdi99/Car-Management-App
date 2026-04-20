namespace System;

using ConsoleTables;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using otherFile;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Car
{

    public String? Make { get; set; }
    public String? Model { get; set; }
    public String? Year { get; set; }
    public double Price { get; set; }
    public Guid Id { get; }
    public static List<Car> carList = new List<Car>();

    [JsonConstructor]
    public Car(String make, String model, String year, double price)
    {
        Make = make;
        Model = model;
        Year = year;
        Price = price;
        Id = Guid.NewGuid();
    }
    public Car(Car car)
    {
        Make = car.Make;
        Model = car.Model;
        Year = car.Year;
        Price = car.Price;
        Id = car.Id;
    }

    // public double generateInvoice(int? warranty)
    // {
    //     var salesTax = 0.725;
    //     if (warranty == null)
    //     {
    //         Invoice = Price * salesTax;
    //     }
    //     else Invoice = Price * salesTax * (warranty * (Price * 0.15));
    //     return (double)Invoice;
    // }

    public String display()
    {
        return $"ID: {Id}\nMAKE: {Make}\nMODEL: {Model}\nPrice: {Price:C}";
    }
    public static void updateList(Car car)
    {
        carList.Add(car);
    }

}
class Program
{
    static async Task Main(string[] args)
    {
        var carList = new List<Car>();
        Car dummyCar = new Car("Honda", "Civic", "2020", 10000.00);
        carList.Add(dummyCar);
        Console.WriteLine("Greetings! Welcome to the car management system!");
        var path = Path.Combine(Directory.GetCurrentDirectory(), "CarJSON.txt");
        var LineRead = File.ReadAllLines(path);

        if (!File.Exists(path))
        {
            File.CreateText(path);
        }

        foreach (var line in LineRead)
        {
            carList.Add(JsonSerializer.Deserialize<Car>(line)!);
        }
        // using(StreamReader sr = new StreamReader(path))
        // {
        //     string? line;
        //     while((line = await sr.ReadLineAsync()) != null)
        //     {
        //         Car? car = JsonSerializer.Deserialize<Car>(line);
        //     }
        // }

        bool running = true;

        while (running)
        {
            Console.WriteLine(@"Please enter a number for the corresponding service you would like to use 
1. Enter new Car
2. List Cars
3. Edit Car
4. Search for a Car
5. Exit");

            String option = Console.ReadLine()!;
            if (option == "1")
            {
                Console.WriteLine("Enter Make:");
                String make = Console.ReadLine()!;

                Console.WriteLine("Enter Model:");
                String model = Console.ReadLine()!;

                Console.WriteLine("Enter Year:");
                String year = Console.ReadLine()!;

                Console.WriteLine("Enter Price:");
                double price = double.Parse(Console.ReadLine()!);

                Car tempCar = new Car(make, model, year, price);
                carList.Add(tempCar);

                String jsonLine = JsonSerializer.Serialize(tempCar);
                await File.AppendAllTextAsync(path, jsonLine + Environment.NewLine);

                continue;
            }
            else if (option == "2")
            {
                var cTable = new ConsoleTable("ID", "Make", "Model", "Year", "Price");
                foreach (Car car in carList)
                {
                    cTable.AddRow(car.Id, car.Make, car.Model, car.Year, car.Price);
                    cTable.Write();
                }


            }
            else if (option == "3")
            {
                Console.WriteLine("Would you like to update or delete a car? Enter 1 to delete, 2 to update:");
                String temp = Console.ReadLine()!;
                if (temp == "1")
                {

                    Console.WriteLine("Enter the Id of the car you want to delete:");
                    Guid deleteId = Guid.Parse(Console.ReadLine()!);
                    Car carToDel = carList.Find(delCar => delCar.Id == deleteId)!;
                    carList.Remove(carToDel);

                    var lines = carList.Select(c => JsonSerializer.Serialize(c));
                    await File.WriteAllLinesAsync(path, lines);
                }
                else if (temp == "2")
                {

                    Console.WriteLine("Enter the Id of the car you want to edit:");
                    Guid editId = Guid.Parse(Console.ReadLine()!);
                    Car carToUpdate = carList.Find(editCar => editCar.Id == editId)!;
                    Console.WriteLine("Enter the New Make:");
                    String make = Console.ReadLine()!;

                    Console.WriteLine("Enter the new Model:");
                    String model = Console.ReadLine()!;

                    Console.WriteLine("Enter the new Year:");
                    String year = Console.ReadLine()!;

                    Console.WriteLine("Enter the new Price:");
                    double price = double.Parse(Console.ReadLine()!);

                    carToUpdate.Make = make;
                    carToUpdate.Model = model;
                    carToUpdate.Year = year;
                    carToUpdate.Price = price;

                    var updatedJsonLine = carList.Select(c => JsonSerializer.Serialize(c));
                    File.WriteAllLines(path, updatedJsonLine);
                }
                else
                {
                    continue;
                }

            }
            else if (option == "4")
            {
                Console.WriteLine("Enter the car year:");
                var year = Console.ReadLine();
                var carsOfACertainYear = carList.Where(c => c.Year == year).ToList();
                var table = new ConsoleTable("ID", "Make", "Model", "Year", "Price");
                foreach (Car car in carsOfACertainYear)
                {
                    table.AddRow(car.Id, car.Make, car.Model, car.Year, car.Price);
                    table.Write();
                }

            }
            // {
            //     Console.WriteLine("Enter the Guid of the car you would like to generate an invoice for:");
            //     Guid invoiceId = Guid.Parse(Console.ReadLine()!);
            //     Car carToAddInvoice = carList.Find(invoiceCar => invoiceCar.Id == invoiceId)!;
            //     Console.WriteLine("Select a length (in years) of a warranty. Leave blank if you do not want a warranty plan.");
            //     int? temp = int.Parse(Console.ReadLine()!);
            //     Console.WriteLine($"Invoice for {carToAddInvoice.Id}: {carToAddInvoice.generateInvoice(temp)}");
            // }
            // else if (option == "5")
            // {
            //     Console.WriteLine("Enter the Guid of the car you would like to view the invoice for:");
            //     Guid invoiceId = Guid.Parse(Console.ReadLine()!);
            //     Car carToAddInvoice = carList.Find(invoiceCar => invoiceCar.Id == invoiceId)!;
            //     Console.WriteLine($"ID: {carToAddInvoice.Id} Invoice: {carToAddInvoice?.Invoice}");
            //}
            else if (option == "5")
            {
                running = false;
            }

            //else Console.WriteLine("Please enter a valid number--1, 2, or 3.");
        }
    }
}
