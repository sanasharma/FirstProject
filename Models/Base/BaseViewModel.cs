using System;
using Newtonsoft.Json;

namespace Models.Base
{
    public class BaseViewModel
    {
        public Result Result { get; set; }

        public BaseViewModel()
        {
            Result = new Result();
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}