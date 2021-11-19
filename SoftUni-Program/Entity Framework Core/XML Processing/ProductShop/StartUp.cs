using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

           // string inputXml = File.ReadAllText("../../../Datasets/categories-products.xml");

            System.Console.WriteLine(GetSoldProducts(context));

        }
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Users");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserInputDto[]), xmlRoot);

            using (StringReader stringReader = new StringReader(inputXml))
            {
                UserInputDto[] dtos = (UserInputDto[])xmlSerializer.Deserialize(stringReader);

                ICollection<User> users = new HashSet<User>();

                foreach (var dtoUser in dtos)
                {
                    User user = new User()
                    {
                        FirstName = dtoUser.FirstName,
                        LastName = dtoUser.LastName,
                        Age = dtoUser.Age
                    };
                    users.Add(user);
                }
                context.Users.AddRange(users);
                context.SaveChanges();
                return $"Successfully imported {users.Count}";
            }
          
        }
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Products");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProductImportDto[]), xmlRoot);

            using (StringReader stringReader = new StringReader(inputXml))
            {
                ProductImportDto[] dtosProducts = (ProductImportDto[])xmlSerializer.Deserialize(stringReader);

                ICollection<Product> products = new HashSet<Product>();

                foreach (var productDto in dtosProducts)
                {
                    Product product = new Product()
                    {
                        Name = productDto.Name,
                        Price = decimal.Parse(productDto.Price),
                        SellerId = productDto.SellerId,
                        BuyerId = productDto.BuyerId
                    };
                    products.Add(product);
                }
                context.Products.AddRange(products);
                context.SaveChanges();
                return $"Successfully imported {products.Count}";
            }
        }
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Categories");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CategoryImportDto[]), xmlRoot);

            using (StringReader stringReader = new StringReader(inputXml))
            {
                CategoryImportDto[] dtosCategories = (CategoryImportDto[])xmlSerializer.Deserialize(stringReader);

                ICollection<Category> categories = new HashSet<Category>();

                foreach (var categoryDto in dtosCategories)
                {
                    if (categoryDto.Name == null)
                    {
                        continue;
                    }
                    Category category = new Category()
                    {
                       Name = categoryDto.Name
                    };
                    categories.Add(category);
                }
                context.Categories.AddRange(categories);
                context.SaveChanges();
                return $"Successfully imported {categories.Count}";
            }
        }
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("CategoryProducts");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CategoryProductImportDto[]), xmlRoot);

            using (StringReader stringReader = new StringReader(inputXml))
            {
                CategoryProductImportDto[] dtosCategoryProduct = (CategoryProductImportDto[])xmlSerializer.Deserialize(stringReader);

                ICollection<CategoryProduct> categoriesProducts = new HashSet<CategoryProduct>();

                foreach (var categoryProductDto in dtosCategoryProduct)
                {
                    Category category = context.Categories.Find(categoryProductDto.CategoryId);
                    Product product = context.Products.Find(categoryProductDto.ProductId);

                    if (category == null || product == null)
                    {
                        continue;
                    }
                    CategoryProduct categoryProduct = new CategoryProduct()
                    {
                       Category = category,
                       Product = product
                    };
                    categoriesProducts.Add(categoryProduct);
                }
                context.CategoryProducts.AddRange(categoriesProducts);
                context.SaveChanges();
                return $"Successfully imported {categoriesProducts.Count}";
            }
        }
        public static string GetProductsInRange(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Products");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProductExportDto[]), xmlRoot);


            using (StringWriter stringWriter = new StringWriter(sb))
            {
                ProductExportDto[] products = context.Products
                    .Where(p => p.Price >= 500 && p.Price <= 1000)
                    .OrderBy(o => o.Price)
                    .Take(10)
                    .Select(s => new ProductExportDto
                    { 
                        Name = s.Name,
                        Price = s.Price.ToString(),
                        Buyer = $"{s.Buyer.FirstName} {s.Buyer.LastName}" ?? s.Buyer.LastName
                    })
                    .ToArray();

                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(string.Empty, string.Empty);
              

                xmlSerializer.Serialize(stringWriter, products, xmlSerializerNamespaces);
            
            }
            return sb.ToString().TrimEnd();
        }
        public static string GetSoldProducts(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Users");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportUsersSoldProductsDto[]), xmlRoot);


            using (StringWriter stringWriter = new StringWriter(sb))
            {
                ExportUsersSoldProductsDto[] users = context.Users
                    .Where(x => x.ProductsSold.Any())
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .Select(s => new ExportUsersSoldProductsDto
                    {
                        FirstName = s.FirstName != null ? s.FirstName : string.Empty,
                        LastName = s.LastName,
                        SoldProducts = s.ProductsSold.Select( y => new ProductExportDto
                        { 
                             Name = y.Name,
                             Price = y.Price.ToString()
                        }).ToArray()

                    }).Take(5).ToArray();

                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(string.Empty, string.Empty);


                xmlSerializer.Serialize(stringWriter, users, xmlSerializerNamespaces);

            }
            return sb.ToString().TrimEnd();
        }


    }
}







    
