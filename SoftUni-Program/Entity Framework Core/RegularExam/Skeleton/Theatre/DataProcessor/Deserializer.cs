namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";

        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            XmlRootAttribute root = new XmlRootAttribute("Plays");
            XmlSerializer serializer = new XmlSerializer(typeof(importPlayDto[]), root);

            StringBuilder sb = new StringBuilder();
            using StringReader sr = new StringReader(xmlString);

            importPlayDto[] plays = (importPlayDto[])serializer.Deserialize(sr);
            List<Play> importPlays = new List<Play>();

            foreach (importPlayDto playdto in plays)
            {
                if (!IsValid(playdto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                object genre;
                if (!Enum.TryParse(typeof(Genre), playdto.Genre, out genre))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                if (TimeSpan.Parse(playdto.Duration).TotalHours < 1)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;

                }
                Play play = new Play()
                {
                    Title = playdto.Title,
                    Duration = TimeSpan.Parse(playdto.Duration),
                    Genre = (Genre)genre,
                    Rating = playdto.Rating,
                    Description = playdto.Description,
                    Screenwriter = playdto.Screenwriter

                };

                importPlays.Add(play);
                sb.AppendLine(string.Format(SuccessfulImportPlay, playdto.Title, playdto.Genre, playdto.Rating));

            }
            context.Plays.AddRange(importPlays);
            context.SaveChanges();
            return sb.ToString().TrimEnd();

        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {

            XmlRootAttribute root = new XmlRootAttribute("Casts");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportCastsDto[]), root);

            StringBuilder sb = new StringBuilder();


            using StringReader sr = new StringReader(xmlString);

            ImportCastsDto[] casts = (ImportCastsDto[])serializer.Deserialize(sr);

            List<Cast> importCast = new List<Cast>();
            foreach (ImportCastsDto cast in casts)
            {
                if (!IsValid(cast))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;

                }
                Cast imCast = new Cast()
                {
                    FullName = cast.FullName,
                    IsMainCharacter = bool.Parse(cast.IsMainCharacter),
                    PhoneNumber = cast.PhoneNumber,
                    PlayId = cast.PlayId
                };
                string role = "main";
                if (imCast.IsMainCharacter == false)
                {
                    role = "lesser";
                }

                importCast.Add(imCast);
                sb.AppendLine(string.Format(SuccessfulImportActor, cast.FullName, role));
            }
            context.Casts.AddRange(importCast);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            ImportProjectionsDto[] theatersImport = JsonConvert.DeserializeObject<ImportProjectionsDto[]>(jsonString);

            List<Theatre> theatres = new List<Theatre>();

            foreach (ImportProjectionsDto theatImport in theatersImport)
            {
                if (!IsValid(theatImport))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Theatre theatre = new Theatre()
                {
                    Name = theatImport.Name,
                    NumberOfHalls = theatImport.NumberOfHalls,
                    Director = theatImport.Director,

                };

                foreach (var ticket in theatImport.Tickets)
                {
                    Play play = context.Plays.Where(w => w.Id == ticket.PlayId).FirstOrDefault();

                    if (play == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        break;
                    }

                    if (!IsValid(ticket))
                    {

                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    Ticket tickettoimport = new Ticket()
                    {
                        Price = ticket.Price,
                        RowNumber = ticket.RowNumber,
                        PlayId= ticket.PlayId
                        
                    };
                    theatre.Tickets.Add(tickettoimport);
                }
                theatres.Add(theatre);
                sb.AppendLine(string.Format(SuccessfulImportTheatre, theatre.Name, theatre.Tickets.Count));
            }
            context.Theatres.AddRange(theatres);
            context.SaveChanges();
            return sb.ToString().TrimEnd();

        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
