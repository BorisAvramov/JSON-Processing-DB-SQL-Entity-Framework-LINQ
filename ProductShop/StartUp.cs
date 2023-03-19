using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.Xml;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();


            // 01. Query 1. Import Users


            //string inputUsersJson = File.ReadAllText("../../../Datasets/users.json");

            //string result1 = ImportUsers(context, inputUsersJson);
            //Console.WriteLine(result1);

            //==================================================================================

            // 02. Query 2. Import Products

            //string inputProductsJson = File.ReadAllText("../../../Datasets/products.json");

            //string result2 = ImportProducts(context, inputProductsJson);

            //Console.WriteLine(result2);

            //==================================================================================
            // 03. Query 3. Import Categories

            //string inputCategoryJson = File.ReadAllText("../../../Datasets/categories.json");

            //string result3 = ImportCategories(context, inputCategoryJson);

            //Console.WriteLine(result3);
            // ==================================================================================
            //04. Query 4. Import Categories and Products

            //string ImportCategoriesProductsJson = File.ReadAllText("../../../Datasets/categories-products.json");

            //string result4 = ImportCategoryProducts(context, ImportCategoriesProductsJson);

            //Console.WriteLine(result4);

            // ===================================================================================
            // 05. Export Products in Range

            // HERE THERE ARE SEVERAL APPROACHES:
            //1. Anonymous object + Manual Mapping
            //2 DTO + Manual Mapping
            //3. DTO + Auto MApping

            //string result5 = GetProductsInRange(context);
            //File.WriteAllText("../../../Results/export-products-in-range.json", result5);
            //Console.WriteLine(result5);

            //=====================================================================================

            // 06. Export Sold Products

            //string result6 = GetSoldProducts(context);
            //File.WriteAllText("../../../Results/export-users-with-sold-roducts.json", result6);

            //Console.WriteLine(result6);

            // 07. Export Categories by Products Count

            //string result7 = GetCategoriesByProductsCount(context);
            //File.WriteAllText("../../../Results/export-categories-products-counts-average-totalrevenue.json", result7);
            //Console.WriteLine(result7);
            // ====================================================================================
            // 08. Export Users and Products

            string result8 = GetUsersWithProducts(context);
            File.WriteAllText("../../../Results/export-users-usercount-with-sold-products.json", result8);
            Console.WriteLine(result8);


        }
        private static IMapper CreateMapper()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            IMapper mapper = new Mapper(mapperConfiguration);

            return mapper;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .OrderByDescending(u => u.ProductsSold.Count(p => p.BuyerId != null))
                .Select(u => new
                {   // UserDto
                    

                    firstName = u.FirstName,
                     
                    lastName = u.LastName,
                      
                    age = u.Age,
                    soldProducts =  new
                    { // ProductWrapperDto
                        count = u.ProductsSold.Count(p => p.BuyerId != null),
                        products = u.ProductsSold
                            .Where(P => P.BuyerId != null)
                            .Select(p => new
                        { //ProductDto
                            name = p.Name,
                            price = p.Price
                        })
                        .ToArray()
                       
                    }
                    
                })
                .ToArray();
            //UserWrapperDto 
            var userWrapperDto = new
            {
                usersCount = users.Length,
                users = users
            }; 

            return JsonConvert.SerializeObject(userWrapperDto, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });

        }

        


        static string GetCategoriesByProductsCount(ProductShopContext context)
        {
             var categories = context.Categories
                .OrderByDescending(c => c.CategoriesProducts.Count)
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoriesProducts.Count,
                    averagePrice = (c.CategoriesProducts.Any()
                        ? c.CategoriesProducts.Average(p => p.Product.Price)
                        : 0).ToString("f2"),
                    totalRevenue = (c.CategoriesProducts.Any()
                        ? c.CategoriesProducts.Sum(p => p.Product.Price)
                        : 0).ToString("f2")


                })
                .AsNoTracking()
                .ToList();

            return JsonConvert.SerializeObject(categories, Newtonsoft.Json.Formatting.Indented);



            //IMapper mapper = CreateMapper();

            //ExportCategorieWithProductsCounsAveragePriceTotalRevenue[] categories = context.Categories
            //    .OrderByDescending(c => c.CategoriesProducts.Count)
            //    .ProjectTo<ExportCategorieWithProductsCounsAveragePriceTotalRevenue>(mapper.ConfigurationProvider)
            //    .ToArray();


            //return JsonConvert.SerializeObject(categories, Newtonsoft.Json.Formatting.Indented);

        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            //1 way A + Manual Mapping

            //var usersWithSoldProducts = context.Users
            //    .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
            //    .OrderBy(u => u.LastName)
            //    .ThenBy(u => u.FirstName)
            //    .Select(u => new
            //    {
            //        firstName = u.FirstName,
            //        lastName = u.LastName,
            //        soldProducts = u.ProductsSold
            //            .Select(p => new
            //            {
            //                name = p.Name,
            //                price = p.Price,
            //                buyerFirstName = p.Buyer.FirstName,
            //                buyerLastName = p.Buyer.LastName
            //            })
            //            .ToList()
            //    })
            //    .ToList();

            //return JsonConvert.SerializeObject(usersWithSoldProducts, Newtonsoft.Json.Formatting.Indented);



            IMapper mapper = CreateMapper();

            var usersWithSoldProductsDto = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ProjectTo<ExportUserWithSoldProductsDto>(mapper.ConfigurationProvider);

            return JsonConvert.SerializeObject(usersWithSoldProductsDto, Newtonsoft.Json.Formatting.Indented);

        }



        public static string GetProductsInRange(ProductShopContext context)
        {
            IMapper mapper = CreateMapper();

            ExportProductDto[] productsTargetDtos = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .AsNoTracking()
                .ProjectTo<ExportProductDto>(mapper.ConfigurationProvider)
                .ToArray();

            return JsonConvert.SerializeObject(productsTargetDtos, Newtonsoft.Json.Formatting.Indented);

            // 1. Anonymous object + Manual Mapping

            //var products = context.Products
            //        .Where(p => p.Price >= 500 && p.Price <= 1000)
            //        .Select(p => new
            //        {
            //            name = p.Name,
            //            price = p.Price,
            //            seller = p.Seller.FirstName + " " + p.Seller.LastName

            //        })
            //        .AsNoTracking()
            //        .ToList();
            //return JsonConvert.SerializeObject(products, Newtonsoft.Json.Formatting.Indented);

        }


        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();

            ImportCategoryProductDto[] importCategoriesProductsDtos = JsonConvert.DeserializeObject<ImportCategoryProductDto[]>(inputJson);




            ICollection<CategoryProduct> validCategoriesProductsModels = new HashSet<CategoryProduct>();


            foreach (var CatProdDto in importCategoriesProductsDtos) 
            {
                // Validation if CategoryId and ProductId exists  in dbSets 
                //but JUDJE DOESNT WORK WITH THAT

                //if ((context.Categories.Any(c => c.Id == CatProdDto.CategoryId)) 
                //    && (context.Products.Any(p => p.Id == CatProdDto.ProductId))
                //    )
                //{
                //    CategoryProduct catagoryProduct = mapper.Map<CategoryProduct>(CatProdDto);

                //    validCategoriesProductsModels.Add(catagoryProduct); 

                //}

                CategoryProduct catagoryProduct = mapper.Map<CategoryProduct>(CatProdDto);

                validCategoriesProductsModels.Add(catagoryProduct);

            }

            context.CategoriesProducts.AddRange(validCategoriesProductsModels);
            context.SaveChanges();

           
            return $"Successfully imported {validCategoriesProductsModels.Count}";



        }


        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();

            ImportCategoryDto[] importCategoriesDtos = JsonConvert.DeserializeObject<ImportCategoryDto[]>(inputJson);


            Category[] categoriesModels = mapper.Map<Category[]>(importCategoriesDtos);

            ICollection<Category> validCategories = new HashSet<Category>();

            foreach (var curCategroie in categoriesModels)
            {
                if (!string.IsNullOrEmpty(curCategroie.Name))
                {
                    validCategories.Add(curCategroie);

                }

            }

            context.Categories.AddRange(validCategories);

            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";


        }



        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            ImportUserDto[] userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson);

            IMapper mapper = CreateMapper();

            ICollection<User> validUsers = new HashSet<User>();

            foreach (var curDto in userDtos)
            {

                User curUser = mapper.Map<User>(curDto);
                validUsers.Add(curUser);

            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();

            return $"Successfully imported {validUsers.Count}";


        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();

            ImportProductDto[] productsDto = JsonConvert.DeserializeObject<ImportProductDto[]>(inputJson);

            Product[] productsModels = mapper.Map<Product[]>(productsDto);

            context.Products.AddRange(productsModels);

            context.SaveChanges();

            return $"Successfully imported {productsModels.Length}";



        }


    }
}