namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context =
                new MusicHubDbContext();


            DbInitializer.ResetDatabase(context);

            //Test your solutions here

            string result = ExportSongsAboveDuration(context, 4);
            Console.WriteLine(result);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {

            StringBuilder sb = new StringBuilder();

            var result = context.Albums
                .ToArray()
                .Where(s => s.ProducerId == producerId)
                .Select(i => new
                {
                    AlbumName = i.Name,
                    AlbumPrice = i.Price,
                    AlbumRelease = i.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    ProducerName = i.Producer.Name,
                    SongsColection = i.Songs
                    .ToArray()
                        .OrderByDescending(s => s.Name)
                        .ThenBy(w => w.Writer.Name)
                        .Select(s => new
                        {
                            SongName = s.Name,
                            SongWriter = s.Writer.Name,
                            SongPrice = s.Price
                        }
                        )
                        .ToArray()
                })
                .OrderByDescending(o => o.AlbumPrice)
                .ToArray();


            foreach (var album in result)
            {
                sb.AppendLine($"-AlbumName: {album.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {album.AlbumRelease}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine($"-Songs:");

                int i = 1;

                foreach (var song in album.SongsColection)
                {
                    sb.AppendLine($"---#{i++}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.SongPrice.ToString("f2")}");
                    sb.AppendLine($"---Writer: {song.SongWriter}");
                }

                sb.AppendLine($"-AlbumPrice: {album.AlbumPrice.ToString("f2")}");

            }
            return sb.ToString().TrimEnd();

        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();
            var result = context.Songs
                .ToArray()
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    Name = s.Name,
                    Writer = s.Writer.Name,
                    Producer = s.Album.Producer.Name,
                    Durattion = s.Duration.ToString("c", CultureInfo.InvariantCulture),
                    PerofmerName = s.SongPerformers
                    .ToArray()
                    .Select(sp => $"{sp.Performer.FirstName} {sp.Performer.LastName}")
                    .FirstOrDefault()

                })
                .OrderBy(n => n.Name)
                .ThenBy(w => w.Writer)
                .ThenBy(p => p.PerofmerName)
                .ToArray();

            int i = 1;

            foreach (var song in result)
            {
                sb.AppendLine($"-Song #{i++}");
                sb.AppendLine($"---SongName: {song.Name}");
                sb.AppendLine($"---Writer: {song.Writer}");
                sb.AppendLine($"---Performer: {song.PerofmerName}");
                sb.AppendLine($"---AlbumProducer: {song.Producer}");
                sb.AppendLine($"---Duration: {song.Durattion}");

            }
            return sb.ToString().TrimEnd();
        }
    }
}
