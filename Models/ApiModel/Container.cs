using System;
using System.Collections.Generic;

namespace Models.ApiModel
{
    public class Container
    {
        public string ip { set; get; }
        public List<string> containers { set; get; }
    }
}