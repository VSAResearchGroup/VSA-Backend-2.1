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
            try
            {
                var DBPlugin = new DBConnection();
                var majorId = Convert.ToInt32(content.major);
                var schoolId = Convert.ToInt32(content.school);
                var job = Convert.ToInt32(content.job);
                var enrollment = Convert.ToInt32(content.enrollment);
                var coreCourses = Convert.ToInt32(content.courses);
                var quarters = Convert.ToInt32(content.quarters);
                var creditPerQuarter = Convert.ToInt32(content.credits);
                var summer = content.summer;
                DBPlugin.ExecuteToString($"insert into ParameterSet (MajorId, SchoolId, JobTypeID, TimePreferenceId, QuarterPreferenceId, DateAdded, NumberCoreCoursesPerQuarter, " +
                                         $"MaxNumberOfQuarters, CreditsPerQuarter, SummerPreference, EnrollmentTypeId, Status, LastDateModified) values ({majorId}, {schoolId}, {job}, {1}, {1}, '{DateTime.UtcNow}', {coreCourses}," +
                                         $"{quarters}, {creditPerQuarter}, '{summer}', {enrollment}, {1}, '{DateTime.UtcNow}')");
                var insertedId = DBPlugin.ExecuteToString("SELECT IDENT_CURRENT('ParameterSet')");
                var preferenceId = Convert.ToInt32(insertedId);
                var insertedSchedule = SaveSchedule(preferenceId);
                return insertedSchedule.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }

        private int SaveSchedule(int id)
        {
            var scheduler = new Scheduler.Scheduler(id);
            var schedule = scheduler.CreateSchedule();
            int insertedId=0;
            var model = new ScheduleModel
            {
                Quarters = new List<Quarter>()
            };
            var DBPlugin = new DBConnection();
            try
            {
                DBPlugin.ExecuteToString(
                    $"insert into GeneratedPlan (Name, ParameterSetID, DateAdded, LastDateModified, Status) " +
                    $"Values ('latest', {id}, '{DateTime.UtcNow}', '{DateTime.UtcNow}', {1})");
                var idString = DBPlugin.ExecuteToString("SELECT IDENT_CURRENT('GeneratedPlan')");
                insertedId = Convert.ToInt32(idString);
                model.Id = insertedId;

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
                        Title = $"{DateTime.UtcNow.Year + kvp.Key}-{quarter.Key.ToString()}",
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
                                $"Values ({insertedId}, {quarter.Key}, {DateTime.UtcNow.Year + kvp.Key}, {course.GetCurrentJobProcessing().GetID()}, '{DateTime.UtcNow}', '{DateTime.UtcNow}')");

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
             return insertedId;
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
