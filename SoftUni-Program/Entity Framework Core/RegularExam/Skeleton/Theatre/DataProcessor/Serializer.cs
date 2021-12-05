namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var result = context.Theatres
                 .ToArray()
                 .Where(w => w.NumberOfHalls >= numbersOfHalls && w.Tickets.Count >= 20)
                 .Select(s => new
                 {
                     Name = s.Name,
                     Halls = s.NumberOfHalls,
                     TotalIncome = (s.Tickets.Where(w => w.RowNumber >= 1 && w.RowNumber <= 5).Sum(k => k.Price)).ToString("f2"),
                     Tickets = s.Tickets.Where(w => w.RowNumber >= 1 && w.RowNumber <= 5).OrderByDescending(o => o.Price)
                     .Select(l => new
                     {
                         Price = l.Price.ToString("f2"),
                         RowNumber = l.RowNumber

                     }).ToArray()

                 }).OrderByDescending(o => o.Halls)
                 .ThenBy(t => t.Name)
                 .ToArray();

            string json = JsonConvert.SerializeObject(result, Formatting.Indented);
            return json;
        }

        public static string ExportPlays(TheatreContext context, double rating)
        {
            XmlRootAttribute root = new XmlRootAttribute("Plays");
            XmlSerializer serializer = new XmlSerializer(typeof(ExportXml[]), root);

            StringBuilder sb = new StringBuilder();

            using StringWriter stringWriter = new StringWriter(sb);

            var result = context.Plays.ToArray()
                .Where(w => w.Rating <= rating)
                .Select(s => new ExportXml()
                {
                    Title = s.Title,
                    Duration = s.Duration.ToString("c", CultureInfo.InvariantCulture),
                    Rating = s.Rating.ToString() == "0" ? "Premier": s.Rating.ToString(),
                    Genre = s.Genre.ToString(),

                    Actors = s.Casts.Where(w => w.IsMainCharacter == true).Select(d => new ExportAuthor
                    {
                        FullName = d.FullName,
                        MainCharacter = $"Plays main character in '{s.Title}'."

                    }).OrderByDescending(f => f.FullName)
                    .ToArray()


                }).OrderBy(o=> o.Title)
                .ThenByDescending(t=> t.Genre)
                .ToArray();

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(stringWriter, result, namespaces);

            return sb.ToString().TrimEnd();

        }
    }
}
