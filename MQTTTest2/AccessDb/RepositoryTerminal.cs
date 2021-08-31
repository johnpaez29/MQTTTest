using MQTTTest2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace MQTTTest2.AccessDb
{
    public class RepositoryTerminal : BaseData, IRepositoryTerminal
    {
        public Terminal GetTerminal(string serie) {

            Terminal terminal = new Terminal();
            using (_connectionDb = new SqlConnection(_connectionString)) 
            {
                string query = "SELECT * FROM terminal WHERE serie = @serie";
                using (_command = new SqlCommand(query)) 
                {
                    try
                    {
                        _command.Parameters.AddWithValue("@serie", serie);
                        _command.Connection = _connectionDb;
                        _connectionDb.Open();
                        using (SqlDataReader reader = _command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                terminal.Serie = reader["serie"].ToString();
                                terminal.Nombre = reader["nombre"].ToString();

                            }
                        }

                    }
                    catch (Exception e)
                    {
                        if (_connectionDb.State == ConnectionState.Open)
                            _connectionDb.Close();
                        Console.WriteLine(e.Message);
                    }
                }
            }

            return terminal;
        }
    }
}
