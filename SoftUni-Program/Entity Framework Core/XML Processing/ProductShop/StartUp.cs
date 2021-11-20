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

            System.Console.WriteLine(GetUsersWithProducts(context));

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
                        Price = s.Price,
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
                        SoldProducts = s.ProductsSold.Select(y => new exp
                        { 
                             Name = y.Name,
                             Price = y.Price

                        }).ToList()

                    }).Take(5).ToList();

                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(string.Empty, string.Empty);


                xmlSerializer.Serialize(stringWriter, users, xmlSerializerNamespaces);

            }
            return sb.ToString().TrimEnd();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Categories");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportGategoryDto[]), xmlRoot);

            using (StringWriter stringWriter = new StringWriter(sb))
            {
                ExportGategoryDto[] categories = context.Categories
                    .Select(s => new ExportGategoryDto
                    {
                        Name = s.Name,
                        Count = s.CategoryProducts.Count,
                        AveragePrice = s.CategoryProducts.Average(a=> a.Product.Price),
                        TotalRevenue = s.CategoryProducts.Sum(v => v.Product.Price)

                    })
                    .OrderByDescending(o => o.Count)
                    .ThenBy(o => o.TotalRevenue)
                    .ToArray();

                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);

                xmlSerializer.Serialize(stringWriter, categories, namespaces);
            }
            return sb.ToString().TrimEnd();

        }
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Users");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(mainDto[]), xmlRoot);


            using (StringWriter stringWriter = new StringWriter(sb))
            {


                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(string.Empty, string.Empty);


                var users = context.Users
                    .Where(x => x.ProductsSold.Count > 1)
                    .OrderByDescending(x => x.ProductsSold.Count)
                    .Select(d => new ExportUsersSoldProductsDto
                    {
                        FirstName = d.FirstName,
                        LastName = d.LastName,
                        Age = d.Age,

                        SoldProducts = new SoldProductsDto
                        {
                            Count = d.ProductsSold.Count,
                            Products = d.ProductsSold.Select(z => new exp
                            {
                                Name = z.Name,
                                Price = z.Price

                            }).ToList()

                        }

                    }).ToList();

                mainDto exportDto = new mainDto
                {
                    Count = users.Count,
                    ExportUsers = users.Take(10).ToList()

                };

                xmlSerializer.Serialize(stringWriter, exportDto, xmlSerializerNamespaces);

            }
            return sb.ToString().TrimEnd();
        }

    }
}







    
