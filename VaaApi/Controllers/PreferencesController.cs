using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using Scheduler;

namespace VaaApi.Controllers
{
    using System.Web.Http.Cors;
    using System.Web.Http.Results;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class PreferencesController : ApiController
    {
        // GET api/values
        [SwaggerOperation("Post")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public string Post(CourseObject content)
        {
            return "done";
        }



    }

    public class CourseObject
    {
        public string courses { get; set; }
        public string credits { get; set; }
        public string english { get; set; }
        public string enrollment { get; set; }
        public string evening { get; set; }
        public string job { get; set; }
        public string major { get; set; }
        public string math { get; set; }
        public string quarter { get; set; }
        public string quarters { get; set; }
        public string school { get; set; }
        public string summer { get; set; }
    }
}
