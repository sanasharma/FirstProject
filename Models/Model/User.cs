using System;
using System.Collections.Generic;

namespace Models.Model
{
    public class User
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public bool IsSuper { get; set; }
        public bool IsLock { get; set; }
        public System.DateTime LastLoginDate { get; set; }
        public System.DateTime CreateDate { get; set; }
    }
}
