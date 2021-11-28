namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            XmlRootAttribute xmlRootAttribute = new XmlRootAttribute("Projects");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportProjectsDto[]), xmlRootAttribute);

            using StringReader stringReader = new StringReader(xmlString);

            ImportProjectsDto[] projectsDtos = (ImportProjectsDto[])xmlSerializer.Deserialize(stringReader);

            List<Project> projects = new List<Project>();

            StringBuilder sb = new StringBuilder();

            foreach (var importProject in projectsDtos)
            {
                if (!IsValid(importProject))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;

                }

                bool isValidOpendate = DateTime.TryParseExact(importProject.OpenDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None,  out DateTime openDate);

                if (!isValidOpendate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;

                }

                DateTime? dueDate = null;
                if (!string.IsNullOrWhiteSpace(importProject.DueDate))
                {
                    bool isValidDueDate = DateTime.TryParseExact(importProject.DueDate, "dd/MM/yyyy"
                    , CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDateCheck);


                    if (!isValidDueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;

                    }
                    dueDate = dueDateCheck;
                }
                Project project = new Project()
                {
                    Name = importProject.Name,
                    OpenDate = openDate,
                    DueDate = dueDate,
                };

                List<Task> tasks = new List<Task>();

                foreach (var task in importProject.Tasks)
                {
                    if (!IsValid(task))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;

                    }
                    bool validTaskOpenDate = DateTime.TryParseExact(task.OpenDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime taskOpendate);

                    bool validTaskDueDate = DateTime.TryParseExact(task.DueDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime taskDuedate);

                    if (!validTaskOpenDate || !validTaskDueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (taskOpendate < openDate )
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    if (dueDate.HasValue && taskDuedate > dueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Task t = new Task()
                    {
                        Name = task.Name,
                        OpenDate = taskOpendate,
                        DueDate = taskDuedate,
                        ExecutionType = (ExecutionType) task.ExecutionType,
                        LabelType = (LabelType) task.LabelType
                    };

                    tasks.Add(t);
                    project.Tasks = tasks;
                }
               
                projects.Add(project);
                sb.AppendLine(String.Format(SuccessfullyImportedProject, project.Name, tasks.Count));

            }
            context.Projects.AddRange(projects);
            context.SaveChanges();
            return sb.ToString().TrimEnd();

        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            List<ImportEmployeeDto> importEmployeeDtos = JsonConvert
                .DeserializeObject<List<ImportEmployeeDto>>(jsonString);

            List<Employee> employees = new List<Employee>();

            foreach (var empDto  in importEmployeeDtos)
            {
                if (!IsValid(empDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Employee employee = new Employee()
                {
                    Username = empDto.Username,
                    Phone = empDto.Phone,
                    Email = empDto.Email
                };

                List<EmployeeTask> et = new List<EmployeeTask>();

                foreach (var id in empDto.Tasks.Distinct())
                {
                   
                    var checkTask = context.Tasks.Find(id);

                    if (checkTask == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    EmployeeTask employeeTask = new EmployeeTask()
                    { 
                        TaskId = id,
                        Employee = employee
                    };
                    et.Add(employeeTask);

                }
                employee.EmployeesTasks = et;
                employees.Add(employee);
                sb.AppendLine(String.Format(SuccessfullyImportedEmployee, empDto.Username, et.Count));

            }
            context.Employees.AddRange(employees);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}