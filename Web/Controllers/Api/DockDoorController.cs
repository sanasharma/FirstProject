using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using Tool;
using DataAccess;
using Models.Base;
using Models.ApiModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EPC.Controllers.Api
{
    public class DockDoorController : ApiController
    {
        [HttpPost] [ActionName("ContainerIn")]
        public string PostContainerIn(Container value)
        {
            PostResult r = ApiDataAccess.AddContainer(value);
            return JsonConvert.SerializeObject(r);
        }

        [HttpPost] [ActionName("ContainerOut")]
        public string PostContainerOut(Container value)
        {
            PostResult r = ApiDataAccess.DelContainer(value);
            return JsonConvert.SerializeObject(r);
        }

        [HttpPost] [ActionName("Post")]
        public string Post(PostModel value)
        {
            PostResult r = ApiDataAccess.Post(value);
            return JsonConvert.SerializeObject(r);
        }

        [HttpGet] [ActionName("ClearAlarm")]
        public string GetClearAlarm(string id)
        {
            ApiDataAccess.ClearAlarm(id);
            return "";
        }
    }
}