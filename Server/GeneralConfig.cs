using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class GeneralConfig
    {
            public int Port { get; set; }
            public List<User> Users { get; set; } = new List<User>();
    }
    public class User
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
