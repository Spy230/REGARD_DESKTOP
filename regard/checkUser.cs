using System;
using System.Collections.Generic;
using System.Text;

namespace regard
{
    public class checkUser
    {
        public string Login { get; set; }

        public bool IsAdmin { get; private set; }

        public string Role { get; private set; }

        public string Status => IsAdmin ? "Admin" : "User";

        public checkUser(string login, bool isAdmin, string role)
        {
            Login = login.Trim();
            IsAdmin = isAdmin;
            Role = role;
        }
    }
}
