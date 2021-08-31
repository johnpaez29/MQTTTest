using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace MQTTTest2.AccessDb
{
    public class BaseData
    {
        internal string _connectionString;
        internal SqlConnection _connectionDb;
        internal SqlCommand _command;

        public BaseData() 
        {
            _connectionString = @"Data Source=.\SQLEXPRESS;"+ 
                                  "Initial Catalog=Pruebas;" + 
                                  "Integrated Security=True;" +
                                  "User id=sa;" +
                                  "Password=Jelyjohn31;";
        }
    }
}
