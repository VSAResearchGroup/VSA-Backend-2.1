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

    public class RatingController : ApiController
    {
        // GET api/values
        [SwaggerOperation("Post")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public string Post(JObject content)
        {
            var id = (string) content["id"];
            var rating = (JObject) content["rating"];
            var advisorRating = (string)rating["schedule-rating"];
            var advisorInt = Convert.ToInt32(advisorRating);
            var dbhelper = new DBConnection();
            dbhelper.ExecuteToString($"Update GeneratedPlan Set AdvisorScore={advisorInt} where GeneratedPlanID={id}");
            return "done";
        }



    }

    
}
