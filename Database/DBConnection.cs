using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data;
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
                        //Console.WriteLine(String.Format("{0}",
                        // call the objects from their index
                        //reader[0]));
                        i += 1;
                    }
                }
                conn.Close();
            }

            return playerList;
        }
        public static DataTable GetRPSWinStats()
        {
            DataTable resultsTable = new DataTable();
            var connectionString = @"Server=DESKTOP-AL33GKV\SQLEXPRESS;Database=TestDB;Trusted_Connection=true";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT username, COUNT(*) AS wins
                    FROM [Apple_Bot].[dbo].[RPS_winStats]
                    GROUP BY username
                    ORDER BY wins";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(resultsTable);
                    }
                }
                connection.Close();
            }

            return resultsTable;
        }
        public void AddWinnerRPS(string username)
        {

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = @"Server=DESKTOP-AL33GKV\SQLEXPRESS;Database=Apple_Bot;Trusted_Connection=true";    
                // The connection string matches pool A.
                conn.Open();  

                string query = "INSERT INTO RPS_winStats (username, win_datetime) VALUES (@Username, GETDATE())";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
        public void clearStats()
        {

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = @"Server=DESKTOP-AL33GKV\SQLEXPRESS;Database=Apple_Bot;Trusted_Connection=true";    
                // The connection string matches pool A.
                conn.Open();  

                string query = "DELETE FROM RPS_winStats";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
        public static Tuple<string, string> RandomAppleFacts()
        {
            var applefact = "";
            var applecategory = "";
            var i = 0;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = @"Server=DESKTOP-AL33GKV\SQLEXPRESS;Database=Apple_Bot;Trusted_Connection=true";    
                // The connection string matches pool A.
                SqlCommand commandContent = new SqlCommand("select top 1 Apple_fact_content, Apple_fact_category from Apple_Bot.dbo.apple_facts order by NEWID()", conn);
                conn.Open();  

                SqlDataAdapter da = new SqlDataAdapter(commandContent);
                using (SqlDataReader reader = commandContent.ExecuteReader())
                {
                    // while there is another record present
                    while (reader.Read())
                    {
                        applefact = reader[0].ToString();
                        applecategory = reader[1].ToString();

                        // call the objects from their index
                        
                        // write the data on to the screen
                        //Console.WriteLine(String.Format("{0}",
                        // call the objects from their index
                        //reader[0]));
                        i += 1;
                    }
                }
                conn.Close();
            }
            Tuple<string, string> t = new Tuple<string, string>(applefact,applecategory);

            return t;
        }
    }
}