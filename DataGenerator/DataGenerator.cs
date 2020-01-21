using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataGenerator
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using ApiCore;
    using FluentAssertions;

    [TestClass]
    public class DataGenerator
    {
        [TestMethod]
        public void GenerateData()
        {
            var insertedList = new List<int>();
            var coursePrefList = new List<CourseObject>();
            var courseObj = new CourseObject()
            {
                school = "6",
                credits = "10",
                major = "22",
                quarters = "50",
                enrollment = ((int)Constants.EnrollmentType.FullTime).ToString(),
                job = ((int)Constants.JobType.Unemployed).ToString(),
                summer = "N"
            };
            coursePrefList.Add(courseObj);
            VaryEnrollment(courseObj, coursePrefList);
            VaryJob(courseObj, coursePrefList);
            VaryCredits(courseObj, coursePrefList);
            GeneratePlan(coursePrefList, insertedList);
            insertedList.Should().NotBeEmpty();

        }

        private void VaryCredits(CourseObject courseObj, List<CourseObject> coursePrefList)
        {
            var credits = new List<int>() { 5, 10, 15, 3, 1 };
            foreach (var credit in credits)
            {
                var newCourseObj = new CourseObject()
                {
                    job = courseObj.job,
                    school = courseObj.school,
                    major = courseObj.major,
                    credits = credit.ToString(),
                    summer = courseObj.summer,
                    enrollment = courseObj.enrollment,
                    quarters = courseObj.quarters
                };
                coursePrefList.Add(newCourseObj);
            }
        }

        private void VaryJob(CourseObject courseObj, List<CourseObject> coursePrefList)
        {
            var jobTypes = new List<Constants.JobType>() { Constants.JobType.FullTime, Constants.JobType.PartTime, Constants.JobType.Unemployed };
            foreach (var jobType in jobTypes)
            {
                var newCourseObj = new CourseObject()
                {
                    job = ((int)jobType).ToString(),
                    school = courseObj.school,
                    major = courseObj.major,
                    credits = courseObj.credits,
                    summer = courseObj.summer,
                    enrollment = courseObj.enrollment,
                    quarters = courseObj.quarters
                };
                coursePrefList.Add(newCourseObj);
            }
        }

        private void VaryEnrollment(CourseObject courseObj, List<CourseObject> coursePrefList)
        {
            var enrollmentTypes = new List<Constants.EnrollmentType>() { Constants.EnrollmentType.FullTime, Constants.EnrollmentType.PartTime };
            foreach (var enrollmentType in enrollmentTypes)
            {
                var newCourseObj = new CourseObject()
                {
                    job = courseObj.job,
                    school = courseObj.school,
                    major = courseObj.major,
                    credits = courseObj.credits,
                    summer = courseObj.summer,
                    enrollment = ((int)enrollmentType).ToString(),
                    quarters = courseObj.quarters
                };
                coursePrefList.Add(newCourseObj);
            }
        }

        private void GeneratePlan(List<CourseObject> courseObj, List<int> insertedList)
        {
            foreach (var courseObject in courseObj)
            {

                var insertedId = Preferences.ProcessPreference(courseObject, true);
                insertedId.Should().NotBe(0);
                insertedList.Add(insertedId);



            }


        }
    }
}
