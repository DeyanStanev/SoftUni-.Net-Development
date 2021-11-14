using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DtoModels.InputDto;
using ProductShop.DtoModels.OutputDto;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        private static IMapper mapper;
        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //string jsonUserssInput = File.ReadAllText("../../../Datasets/users.json");
            //Console.WriteLine(ImportUsers(context, jsonUserssInput));

            //string jsonProductsInput = File.ReadAllText("../../../Datasets/products.json");
            //Console.WriteLine(ImportProducts(context, jsonProductsInput));

            //string jsonCategoriesInput = File.ReadAllText("../../../Datasets/categories.json");
            //Console.WriteLine(ImportCategories(context, jsonCategoriesInput));

            //string jsonCategoriesProductsInput = File.ReadAllText("../../../Datasets/categories-products.json");
            //Console.WriteLine(ImportCategoryProducts(context, jsonCategoriesProductsInput));

            //Console.WriteLine(GetProductsInRange(context));
            //Console.WriteLine(GetSoldProducts(context));
            //Console.WriteLine(GetCategoriesByProductsCount(context));
            Console.WriteLine(GetUsersWithProducts(context));

        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            IMapperConfig();

            IEnumerable<UsersInputDto> users = JsonConvert.DeserializeObject<IEnumerable<UsersInputDto>>(inputJson);
            var mappedUsers = mapper.Map<IEnumerable<User>>(users);

            context.Users.AddRange(mappedUsers);
            context.SaveChanges();

            return $"Successfully imported {mappedUsers.Count()}";
        }
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            IMapperConfig();

            IEnumerable<ProductInputDto> products = JsonConvert.DeserializeObject<IEnumerable<ProductInputDto>>(inputJson);
            var mappedProducts = mapper.Map<IEnumerable<Product>>(products);

            context.Products.AddRange(mappedProducts);
            context.SaveChanges();

            return $"Successfully imported {mappedProducts.Count()}";

        }
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            IMapperConfig();

            IEnumerable<CategoryInputDto> categories = JsonConvert.DeserializeObject<IEnumerable<CategoryInputDto>>(inputJson)
                .Where(x => x.Name != null);

            var mappedCategories = mapper.Map<IEnumerable<Category>>(categories);

            context.Categories.AddRange(mappedCategories);
            context.SaveChanges();

            return $"Successfully imported {mappedCategories.Count()}";


        }
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            IMapperConfig();

            IEnumerable<CategoryProductInputDto> categoriesProducts = JsonConvert
                .DeserializeObject<IEnumerable<CategoryProductInputDto>>(inputJson);

            var mappedCategoriesProducts = mapper.Map<IEnumerable<CategoryProduct>>(categoriesProducts);

            context.CategoryProducts.AddRange(mappedCategoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {mappedCategoriesProducts.Count()}";

        }
        public static string GetProductsInRange(ProductShopContext context)
        {
            IMapperConfig();

            var result = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .ProjectTo<ProductsOutputDto>(mapper.ConfigurationProvider)
                .ToList();

            DefaultContractResolver defaultContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = defaultContractResolver
            };
               

            string JsonProductOutput = JsonConvert.SerializeObject(result, jsonSettings);
            return JsonProductOutput;
        }
        public static string GetSoldProducts(ProductShopContext context)
        {
            var resultt = context.Users
                .Include(p=> p.ProductsSold)
                .Where(x => x.ProductsSold.Any(y => y.Buyer != null))
                .OrderBy(o => o.LastName)
                .ThenBy(t => t.FirstName)
                .Select(s => new
                {
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    SoldProducts = s.ProductsSold.Select(p => new
                    {
                        Name = p.Name,
                        Price = p.Price,
                        BuyerFirstName = p.Buyer.FirstName,
                        BuyerLastName = p.Buyer.LastName

                    }).ToList()

                }).ToList();

            DefaultContractResolver defaultContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = defaultContractResolver
            };


            string JsonSoldProductsOutput = JsonConvert.SerializeObject(resultt, jsonSettings);
            return JsonSoldProductsOutput;

        }
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var result = context.Categories
              .OrderByDescending(o => o.CategoryProducts.Count)
              .Select(s => new
              {
                  Category = s.Name,
                  ProductsCount = s.CategoryProducts.Count,
                  AveragePrice = $"{s.CategoryProducts.Sum(sp => sp.Product.Price) / s.CategoryProducts.Count:f2}",
                  TotalRevenue = $"{s.CategoryProducts.Sum(sp => sp.Product.Price):f2}"
              }).ToList();

            DefaultContractResolver defaultContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = defaultContractResolver
            };


            string JsonProductsByCountOutput = JsonConvert.SerializeObject(result, jsonSettings);
            return JsonProductsByCountOutput;
        }
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var result = context.Users
            .Where(x => x.ProductsSold.Any(y => y.Buyer != null))
            .OrderByDescending(o => o.ProductsSold.Count)
            .Select(x => new
            {
                
                FirstName = x.FirstName,
                LastName = x.LastName,
                Age = x.Age ,

                SoldProducts = x.ProductsSold.Select(f => new
                {
                    Count = x.ProductsSold.Count(),

                    Products = x.ProductsSold.Select(p => new
                    {
                        Name = p.Name,
                        Price = p.Price
                    })

                })
               
            }).ToList();

            DefaultContractResolver defaultContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = defaultContractResolver,
                NullValueHandling = NullValueHandling.Ignore
            };

            var resultWithCount = new
            {
                usersCount = result.Count,
                result
            };

            string jsonSoldProductsOutput = JsonConvert.SerializeObject(resultWithCount, jsonSettings);
            return jsonSoldProductsOutput;
        }
        private static void IMapperConfig()
        {
            var mapperCongif = new MapperConfiguration(cfg => cfg.AddProfile<ProductShopProfile>());
            mapper = new Mapper(mapperCongif);
            
        }

    }
}