using System;
using Newtonsoft.Json;

namespace Models.Base
{
    public class Result
    {
        public ResultCode Code { set; get; }
        public string Msg { set; get; }

        public Result()
        {
            Code = ResultCode.Null;
        }
        public void Set(ResultCode Code, string Msg)
        {
            this.Code = Code;
            this.Msg = Msg;
        }
    }

    public enum ResultCode { Null, Success, Error, Info, Warning }
}