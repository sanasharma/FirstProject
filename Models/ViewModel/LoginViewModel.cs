using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Models.ViewModel
{
    public class LoginViewModel : Base.BaseViewModel
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string IP { get; set; }
    }
}