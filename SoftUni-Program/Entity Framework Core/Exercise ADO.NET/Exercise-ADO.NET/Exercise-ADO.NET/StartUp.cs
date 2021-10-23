using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace Exercise_ADO.NET
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SqlConnection sqlConnection = new SqlConnection(Connections.CONNECTION_DB);

            sqlConnection.Open();

            using (sqlConnection)
            {
                GetViliansById(sqlConnection, 3);

            };
        }
        //task02
        private static void GetViliansWithMorethan3MInions(SqlConnection sqlConnection)
        {
            SqlCommand sqlCommand = new SqlCommand(Queries.ViliansWithMoreThan3Minions, sqlConnection);
            
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            using (sqlDataReader)
            {
                while (sqlDataReader.Read())
                {
                    string name = sqlDataReader.GetString(0);
                    int count = sqlDataReader.GetInt32(1);
                    Console.WriteLine($"{name} - {count}");
                }
            }
        }
        //task03
        private static void GetViliansById(SqlConnection sqlConnection, int vilianId)
        {
            SqlCommand getVilianName = new SqlCommand(@"Select name from villains where id = @id", sqlConnection);
            getVilianName.Parameters.AddWithValue("@Id", vilianId);
            string vilianName = (string)getVilianName.ExecuteScalar();

            SqlCommand sqlCommand = new SqlCommand(Queries.GetViliansById, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Id", vilianId);
            
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            using (sqlDataReader)
            {
                if (!sqlDataReader.HasRows)
                {
                    Console.WriteLine("No Minions");
                }
                else
                {
                    Console.WriteLine(vilianName);
                    while (sqlDataReader.Read())
                    {
                        long rowNUm = sqlDataReader.GetInt64(0);
                        string name = sqlDataReader.GetString(1);
                        int age = sqlDataReader.GetInt32(2);
                        Console.WriteLine($"{rowNUm} {name} {age}");

                    }
                }


            }
        }
        
    }
}
