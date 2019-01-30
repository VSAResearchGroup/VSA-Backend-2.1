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
    using System.Web.Http.Results;
    using Newtonsoft.Json;

    public class SchedulesController : ApiController
    {
        // GET api/values
        [SwaggerOperation("GetAll")]
        public string Get()
        {
            var schedule = Driver.MakeSchedule();
            var model = new ScheduleModel
            {
                Quarters = new List<Quarter>()
            };
            var byYear = schedule.GroupBy(s => s.GetYear());
            foreach (var kvp in byYear)
            {
                var byQuarter = kvp.GroupBy(s => s.GetQuarter());
                foreach (var quarter in byQuarter)
                {
                    var quarterItem = new Quarter
                    {
                        Year = kvp.Key,
                        Title = kvp.First().GetQuarter().ToString(),
                        Id = $"{kvp.Key}-{quarter.Key}",
                        Courses = new List<Course>()
                    };
                    model.Quarters.Add(quarterItem);

                    foreach (var course in quarter)
                    {
                        var description = course.GetCurrentJobProcessing().GetID().ToString();
                        quarterItem.Courses.Add(new Course() { Description = description, Id = description, Title = description });
                    }

                }
            }

            var response = JsonConvert.SerializeObject(model);
            return response;
        }

    }
}
