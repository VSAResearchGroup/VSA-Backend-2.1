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
                return insertedId.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
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
