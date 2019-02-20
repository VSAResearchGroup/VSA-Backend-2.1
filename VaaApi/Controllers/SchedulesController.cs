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

    public class SchedulesController : ApiController
    {
        // GET api/values
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public string Get(int id)
        {
            var scheduler = new Scheduler.Scheduler(id);
            var schedule = scheduler.CreateSchedule();
            var model = new ScheduleModel
            {
                Quarters = new List<Quarter>()
            };
            int generatedPlanId = 0;
            var DBPlugin = new DBConnection();
            try
            {
                DBPlugin.ExecuteToString(
                    $"insert into GeneratedPlan (Name, ParameterSetID, DateAdded, LastDateModified, Status) " +
                    $"Values ('latest', {id}, '{DateTime.UtcNow}', '{DateTime.UtcNow}', {1})");
                var insertedId = DBPlugin.ExecuteToString("SELECT IDENT_CURRENT('GeneratedPlan')");
                generatedPlanId = Convert.ToInt32(insertedId);
                model.Id = generatedPlanId;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var byYear = schedule.GroupBy(s => s.GetYear());
            foreach (var kvp in byYear)
            {
                var byQuarter = kvp.GroupBy(s => s.GetQuarter());
                foreach (var quarter in byQuarter)
                {
                    var quarterItem = new Quarter
                    {
                        Year = kvp.Key,
                        Title = $"{DateTime.UtcNow.Year+ kvp.Key}-{quarter.Key.ToString()}",
                        Id = $"{kvp.Key}-{quarter.Key}",
                        Courses = new List<Course>()
                    };
                    model.Quarters.Add(quarterItem);

                    foreach (var course in quarter)
                    {
                        var description = course.GetCurrentJobProcessing().GetID().ToString();
                        quarterItem.Courses.Add(new Course() { Description = description, Id = description, Title = description });

                        try
                        {
                            DBPlugin.ExecuteToString(
                                $"insert into StudyPlan (GeneratedPlanID, QuarterID, YearID, CourseID, DateAdded, LastDateModified) " +
                                $"Values ({generatedPlanId}, {quarter.Key}, {DateTime.UtcNow.Year+kvp.Key}, {course.GetCurrentJobProcessing().GetID()}, '{DateTime.UtcNow}', '{DateTime.UtcNow}')");

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                       
                    }

                }
            }
            //save the plan if needed


            var response = JsonConvert.SerializeObject(model);
            return response;
        }

    }
}
