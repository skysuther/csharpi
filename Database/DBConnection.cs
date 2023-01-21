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