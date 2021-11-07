namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();

            DbInitializer.ResetDatabase(db);


           
            Console.WriteLine(RemoveBooks(db));
        }

        //Task 01 - Age restirction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();

            AgeRestriction ageRestriction = Enum.Parse<AgeRestriction>(command, true);
            string[] titles = context.Books
                                .Where(b => b.AgeRestriction == ageRestriction)
                                .OrderBy(b => b.Title)
                                .Select(b => b.Title)
                                .ToArray();
            foreach (string title in titles)
            {
                sb.AppendLine(title);
            }
            return sb.ToString().TrimEnd();
        }

        //Task 02 - Golden books
        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            string[] result = context.Books
                .ToArray()
                .Where(b => b.EditionType.ToString() == "Gold" && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();
            foreach (var titile in result)
            {
                sb.AppendLine(titile);

            }
            return sb.ToString().TrimEnd();

        }

        //Task 03 - Books by price
        //TODO retest, test fail
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var result = context.Books
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(s => new
                {
                    Title = s.Title,
                    Price = s.Price
                })
                .ToArray();

            foreach (var book in result)
            {
                sb.AppendLine($"{book.Title} - ${book.Price}");
            }
            return sb.ToString().TrimEnd();
        }

        //Task 04 - Not released in
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder sb = new StringBuilder();

            var result = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(s => s.Title)
                .ToArray();

            foreach (var book in result)
            {
                sb.AppendLine(book);
            }
            return sb.ToString().TrimEnd();
        }

        //Task 05 - Books titles by category

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            string[] categories = input.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var result = context.BooksCategories
              .Select(b => new
              {
                  Category = b.Category.Name,
                  Title = b.Book.Title
              })
              .OrderBy(b => b.Title)
              .ToArray();

            foreach (var book in result)
            {
                if (categories.Contains(book.Category.ToLower()))
                {
                    sb.AppendLine(book.Title);
                }
                
            }

            return sb.ToString().TrimEnd();
        }

        //Task 06 
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();

            DateTime inputDate = DateTime.Parse(date);

            var result = context.Books
                .ToArray()
                .OrderByDescending(o => o.ReleaseDate)
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate < inputDate)
                .Select(s => new
                {
                    Title = s.Title,
                    EditionType = s.EditionType.ToString(),
                    Price = s.Price
                })
                .ToArray();
            foreach (var book in result)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price}");
            }
            return sb.ToString().TrimEnd();

        }

        //Task 07

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var result = context.Authors
                .ToArray()
                .Where(b => b.FirstName.ToLower().EndsWith(input.ToLower()))
                .OrderBy(b => b.FirstName)
                .Select(s => new 
                { 
                    firstName = s.FirstName,
                    lastname = s.LastName
                })
                .ToArray();
            foreach (var autor in result)
            {
                sb.AppendLine($"{autor.firstName} {autor.lastname}");
            }
            return sb.ToString().TrimEnd();
        }

        //Task 08
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            var result = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .OrderBy(o => o.Title)
                .ToArray();
            foreach (var book in result)
            {
                sb.AppendLine(book.Title);
            }
            return sb.ToString().TrimEnd();

        }

        //Task 09
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            var result = context.Books
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(o => o.BookId)
                .Select(s=> new 
                {
                    AutorName = s.Author.FirstName != null ?  s.Author.FirstName + " " + s.Author.LastName : s.Author.LastName,
                    BookTitle = s.Title

                })
                .ToList();

            foreach (var book in result)
            {
                sb.AppendLine($"{book.BookTitle} ({book.AutorName})");
            }
            return sb.ToString().TrimEnd();
        }

        //Task 10
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
             int result = context.Books.Where(b => b.Title.Length > lengthCheck).Count();
             return result;
           
        }

        //Task 11
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var result = context.Authors
                .ToArray()
                .Select(s => new
                {
                    AutorName = s.FirstName != null ? s.FirstName + " " + s.LastName : s.LastName,
                    Copies = s.Books.Where(c => c.AuthorId == c.Author.AuthorId)
                    .Select(d => d.Copies).Sum()

                })
                .OrderByDescending(o => o.Copies)
                .ToArray();
            foreach (var autor in result)
            {
                sb.AppendLine($"{autor.AutorName} - {autor.Copies}");
            }
            return sb.ToString().TrimEnd();
        }

        //Task 12
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var result = context.Categories
                .Select(s => new
                {
                    CategoryName = s.Name,
                    Profit = s.CategoryBooks.Where(o => o.CategoryId == o.Category.CategoryId)
                    .Select(s => s.Book.Copies * s.Book.Price).Sum()
                })
                .OrderByDescending(o => o.Profit)
                .ThenBy(t => t.CategoryName)
                .ToArray();

            foreach (var category in result)
            {
                sb.AppendLine($"{category.CategoryName} ${category.Profit:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        //Task 13 
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var result = context.Categories
                .OrderBy(c=> c.Name)
                .Select(s => new
                {
                    CategoryName = s.Name,
                    Books = s.CategoryBooks
                    .Select(b => new
                    {
                        BookTitle = b.Book.Title,
                        Releasedate = b.Book.ReleaseDate,
                        ReleaseYear = b.Book.ReleaseDate.Value.Year.ToString()
                    })
                    .OrderByDescending(x => x.Releasedate)
                    .Take(3)
                    .ToArray()
                })
                .ToArray();
            foreach (var category in result)
            {
                sb.AppendLine($"--{category.CategoryName}");
                foreach (var book in category.Books)
                {
                    sb.AppendLine($"{book.BookTitle} ({book.ReleaseYear})");
                }
            }
            return sb.ToString().TrimEnd();
        }

        //Task 14 
        public static void IncreasePrices(BookShopContext context)
        {
            foreach (var book in context.Books.Where(b=> b.ReleaseDate.Value.Year < 2010))
            {
                book.Price += 5;
            }
            context.SaveChanges();
                
        }

        //Task 15
        public static int RemoveBooks(BookShopContext context)
        {
            var result = context.BooksCategories.Where(b => b.Book.Copies < 4200).ToArray();
            var bookToRemove = context.Books.Where(b => b.Copies < 4200).ToArray();

            context.BooksCategories.RemoveRange(result);
            context.Books.RemoveRange(bookToRemove);
            context.SaveChanges();
            return result.Count();

        }


    }
}
