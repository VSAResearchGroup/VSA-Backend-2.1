using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataGenerator
{
    using ApiCore;
    using FluentAssertions;

    [TestClass]
    public class DataGenerator
    {
        [TestMethod]
        public void GenerateData()
        {
            var courseObj = new CourseObject()
            {
                school = "1",
                credits = "5",
                major = "22",
                quarters = "50",
                enrollment = ((int)Constants.EnrollmentType.FullTime).ToString(), 
                job = ((int)Constants.JobType.Unemployed).ToString(),
                summer = "N"
            };
            var insertedId = Preferences.ProcessPreference(courseObj, false);
            insertedId.Should().NotBe(0);
        }
    }
}
