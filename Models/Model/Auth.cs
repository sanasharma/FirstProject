using System;
using System.Collections.Generic;

namespace Models.Model
{
    public class Auth
    {
        public string MenuNo { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Del { get; set; }
        public bool Query { get; set; }
        public bool Audit { get; set; }
        public bool Print { get; set; }
        public bool Export { get; set; }
        public bool Import { get; set; }
        public bool Admin { get; set; }
        public bool Enabled { get; set; }

        public Auth()
        {
            this.Add = true;
            this.Edit = true;
            this.Del = true;
            this.Query = true;
            this.Audit = true;
            this.Print = true;
            this.Export = true;
            this.Import = true;
            this.Admin = true;
            this.Enabled = true;
        }
    }
}
