namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            XmlRootAttribute xmlRootAttribute = new XmlRootAttribute("Projects");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportProjectsDto[]), xmlRootAttribute);

            StringBuilder sb = new StringBuilder();

            using StringWriter stringWriter = new StringWriter(sb);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            ExportProjectsDto[] exportProjectWithTheirTasks = context.Projects
                .ToArray()
                .Where(t => t.Tasks.Any())
                .Select(s => new ExportProjectsDto()
                {
                    TasksCount = s.Tasks.Count,
                    ProjectName = s.Name,
                    HasEndDate = s.DueDate.HasValue ? "Yes" : "No",
                    Tasks = s.Tasks
                    .Select(f => new ExportTaskDto()
                    {
                        Name = f.Name,
                        Label = f.LabelType.ToString()

                    }).OrderBy(o => o.Name)
                    .ToArray()

                }).OrderByDescending(o=> o.Tasks.Count())
                .ThenBy(t=> t.ProjectName)
                .ToArray();

            xmlSerializer.Serialize(stringWriter, exportProjectWithTheirTasks, namespaces);

            return sb.ToString().TrimEnd();
            
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var exportMostBusiestEmployees = context.Employees
                .ToArray()
                .Where(d => d.EmployeesTasks.Any(t => t.Task.OpenDate >= date))
                .Select(e => new 
                {
                    Username = e.Username,
                    Tasks = e.EmployeesTasks
                    .Where(d => d.Task.OpenDate >= date)
                    .OrderByDescending(o => o.Task.DueDate)
                    .ThenBy(t=> t.Task.Name)
                    .Select (n => new 
                    {
                        TaskName = n.Task.Name,
                        OpenDate = n.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = n.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        LabelType = n.Task.LabelType.ToString(),
                        ExecutionType = n.Task.ExecutionType.ToString()

                    }).ToArray()

                }).OrderByDescending(o => o.Tasks.Count())
                .ThenBy(t => t.Username)
                .Take(10)
                .ToArray();

            string result = JsonConvert.SerializeObject(exportMostBusiestEmployees, Formatting.Indented);
            return result;
        }
    }
}