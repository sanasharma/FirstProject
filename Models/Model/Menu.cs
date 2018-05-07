using System;
using System.Collections.Generic;

namespace Models.Model
{
    public class Menu
    {
        public string MenuNo { get; set; }
        public string MenuName { get; set; }
        public string MenuLink { get; set; }
        public string Type { get; set; }
        public bool isMenu { get; set; }
        public bool isPublic { get; set; }
        public string MenuDesc { get; set; }
        public int OrderID { get; set; }
        public bool Enabled { get; set; }
        public string MenuIco { get; set; }
    }
}
