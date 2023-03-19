using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //09.Import Suppliers

            //string inputSuppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");

            //string result9 = ImportSuppliers(context, inputSuppliersJson);
            //Console.WriteLine(result9);

            //10.Import Parts
            //string inputJsonParts = File.ReadAllText("../../../Datasets/parts.json");
            //string result10 = ImportParts(context, inputJsonParts);
            //Console.WriteLine(result10);
            //11.Import Cars
            //string inputJsonParts = File.ReadAllText("../../../Datasets/cars.json");
            //string result11 = ImportCars(context, inputJsonParts);
            //Console.WriteLine(result11);

            // 12. Import Customers
            // var json = File.ReadAllText("../../../Datasets/customers.json");
            //string result12 =  ImportCustomers(context, json);
            // Console.WriteLine(result12);

            // 13. Import Sales
            //var json = File.ReadAllText("../../../Datasets/sales.json");
            //string result13 = ImportSales(context, json);
            //Console.WriteLine(result13);
            //14 . Export Ordered Customers

            //string json = GetOrderedCustomers(context);
            //File.WriteAllText("../../../Results/ordered-customers.json", json);
            //Console.WriteLine(json);

            //15. Export Cars From Make Toyota
            //string json = GetCarsFromMakeToyota(context);
            //File.WriteAllText("../../../Results/cars-toyota.json", json);
            //Console.WriteLine(json);

            // 16. Export Local Suppliers
            //string json = GetCarsFromMakeToyota(context);
            //File.WriteAllText("../../../Results/local-suppliers.json", json);
            //Console.WriteLine(json);


        }

        private static IMapper CreateMapper()
        {
            MapperConfiguration config = new MapperConfiguration(conf =>
            {
                conf.AddProfile<CarDealerProfile>();
            });

            IMapper mapper = config.CreateMapper();
            return mapper;
        }




        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .ToArray();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

         public static string GetOrderedCustomers(CarDealerContext context)
        {
            var targetCustomers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy",CultureInfo.InvariantCulture),
                    IsYoungDriver = c.IsYoungDriver,
                })
                .ToArray();

            return JsonConvert.SerializeObject(targetCustomers, Formatting.Indented);


        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            ////1.
            //var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

            //var validSales = sales.Where(s => context.Customers.Any(c => c.Id == s.CustomerId)).ToList();

            //context.Sales.AddRange(validSales);


            // 2.

            var mapper = CreateMapper();
            var salesDtos = JsonConvert.DeserializeObject<List<ImportSaleDto>>(inputJson);

            var salesModels = new List<Sale>();
            //var salesModels = mapper.Map<Sale[]>(salesDtos);
            foreach (var dto in salesDtos)
            {
                if (context.Customers.Any(c => c.Id == dto.CustomerId))
                {
                    //Sale sale = new Sale
                    //{
                    //    //CarId = dto.CarId,
                    //    //CustomerId = dto.CustomerId,
                    //    Discount = dto.Discount,
                    //};

                    Sale sale = mapper.Map<Sale>(dto);

                    salesModels.Add(sale);
                }


            }

            context.Sales.AddRange(salesModels);

            context.SaveChanges();

            return $"Successfully imported {salesModels.Count}.";

        }


        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var mapper = CreateMapper();
            var customerDtos = JsonConvert.DeserializeObject<ImportCustomerDto[]>(inputJson);

            var cusomersModels = mapper.Map<Customer[]>(customerDtos);

            context.Customers.AddRange(cusomersModels);

            context.SaveChanges();

            return $"Successfully imported {cusomersModels.Length}.";

        }
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            ImportCarDto[] carsWithPartsDtos = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

            List<Car> CARS = new List<Car>();
            List<PartCar> partscar = new List<PartCar>();

            foreach (var dto in carsWithPartsDtos)
            {
                Car car = new Car 
                {
                    Make = dto.Make,
                    Model = dto.Model,
                    TraveledDistance = dto.TraveledDistance
                };

                foreach (var partId in dto.PartsId.Distinct())
                {
                    car.PartsCars.Add
                        (
                            new PartCar
                            {
                                PartId = partId

                            }
                        
                        );
                }

                CARS.Add(car);
            }

            context.Cars.AddRange(CARS);
            context.SaveChanges();

            return $"Successfully imported {CARS.Count}.";

        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            // 1.
            ////var parts = JsonConvert.DeserializeObject<Part[]>(inputJson);

            ////var validParts = parts.Where(p => context.Suppliers.Any(S => S.Id == p.SupplierId)).ToArray();

            ////context.Parts.AddRange(validParts);

            // 2.
            //var mapper = CreateMapper();

            //List <ImportPartDto> partsDtos = JsonConvert.DeserializeObject<List<ImportPartDto>>(inputJson);

            //var validateParts = new List<Part>();
            //List<int> passedCountId = new List<int>();

            //foreach (var curDto in partsDtos)
            //{

            //    if(!context.Suppliers.Any(s => s.Id == curDto.SupplierId))
            //    {
            //        continue;


            //    }
            //    Part curPart = mapper.Map<Part>(curDto);

            //    validateParts.Add(curPart);

            //}

            //var validateParts = mapper.Map<Part[]>(partsDtos);

            //context.Parts.AddRange(validateParts);

            //3.

            var supplaierId = context.Suppliers.Select(s => s.Id).ToArray();

            var parts = JsonConvert.DeserializeObject<Part[]>(inputJson)
                        .Where(p => supplaierId.Contains(p.SupplierId))
                        .ToArray();
            context.Parts.AddRange(parts);



            context.SaveChanges();
            return $"Successfully imported {parts.Length}.";



        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var mapper = CreateMapper();

            ImportSuplierDto[] suppliersDtos = JsonConvert.DeserializeObject<ImportSuplierDto[]>(inputJson);

            Supplier[] validSuppliers = mapper.Map<Supplier[]>(suppliersDtos);

            context.Suppliers.AddRange(validSuppliers);
            context.SaveChanges();

            return $"Successfully imported {validSuppliers.Length}.";


        }


    }

    
}