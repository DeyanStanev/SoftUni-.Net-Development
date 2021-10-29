﻿using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            string result = GetAddressesByTown(context);
            Console.WriteLine(result);
        }
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
           StringBuilder sb = new StringBuilder();

            var employees = context.Employees.OrderBy( e=> e.EmployeeId).ToArray();
                

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context
                .Employees
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary

                })
                .ToArray();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)

        {

            StringBuilder sb = new StringBuilder();

            var employees = context
                .Employees
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    depName = e.Department.Name,
                    e.Salary
                })
                .ToArray();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.depName} - ${e.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }
        public static string AddNewAddressToEmployee(SoftUniContext context)


        {
            StringBuilder sb = new StringBuilder();
            Address newAddress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };
            context.Addresses.Add(newAddress);
            context.SaveChanges();
            Employee employee = context
                .Employees.FirstOrDefault(e => e.LastName == "Nakov");
            employee.Address = newAddress;
            context.SaveChanges();

            var employees = context
              .Employees
              .OrderByDescending(e => e.AddressId)
              .Take(10)
              .Select(e => new
              {
                  e.AddressId,
                  AddressText = e.Address.AddressText
              })
              .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine(e.AddressText);
            }
            return sb.ToString().TrimEnd();

        }
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context
                .Employees
                .Where(ep => ep.EmployeesProjects.Any(pd => pd.Project.StartDate.Year >= 2001 && pd.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    MangerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,

                    ProjectsList = e.EmployeesProjects.Select(p =>
                    new
                    {
                        ProjectName = p.Project.Name,
                        Startdate = p.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                        EndDate = p.Project.EndDate.HasValue ? p.Project.EndDate.
                        Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture) : "not finished"

                    })
                    .ToList()
                })
                .ToList();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.MangerFirstName} {e.ManagerLastName}");
                foreach (var p in e.ProjectsList)
                {
                    sb.AppendLine($"--{p.ProjectName} - {p.Startdate} - {p.EndDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var result = context.Addresses
                .OrderByDescending(o => o.Employees.Count())
                .ThenBy(t => t.Town.Name)
                .ThenBy(l => l.AddressText)
                .Select(e =>
                new
                {
                    AdressText = e.AddressText,
                    TownName = e.Town.Name,
                    Count = e.Employees.Count()
                })
                .Take(10)
                .ToList();
            foreach (var a in result)
            {
                sb.AppendLine($"{a.AdressText}, {a.TownName} - {a.Count} employees");
            }
            return sb.ToString().TrimEnd();

        }
    }
}
