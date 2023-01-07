using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace csharpi.Connection
{
    public class DBConnection
    {
        public static List<string> RetrievePlayerList()
        {
            var playerList = new List<string>();
            var i = 0;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = @"Server=DESKTOP-AL33GKV\SQLEXPRESS;Database=TestDB;Trusted_Connection=true";    
                // The connection string matches pool A.
                SqlCommand command = new SqlCommand("SELECT PlayerTag FROM Players", conn);
                conn.Open();  

                SqlDataAdapter da = new SqlDataAdapter(command);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // while there is another record present
                    while (reader.Read())
                    {
                        playerList.Add(String.Format("{0}",
                        // call the objects from their index
                        reader[0]));
                        // write the data on to the screen
                        Console.WriteLine(String.Format("{0}",
                        // call the objects from their index
                        reader[0]));
                        i += 1;
                    }
                }
                conn.Close();
            }

            return playerList;
        }
    }
}