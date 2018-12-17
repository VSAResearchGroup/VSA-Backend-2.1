using System;
using Newtonsoft.Json;

namespace Scheduler
{
    public class ParameterSet
    {
        #region Variables
        //------------------------------------------------------------------------------
        // This represents the entirety of ParameterSet in the database. Should the 
        // database be changed then this must also be changed to reflect it for optimum
        // performance
        //------------------------------------------------------------------------------
        [JsonProperty]
        private int ParameterSetID { get; set; }
        [JsonProperty]
        private int MajorID { get; set; }
        [JsonProperty]
        private int SchoolID { get; set; }
        [JsonProperty]
        private int JobTypeID { get; set; }
        [JsonProperty]
        private int TimePreferenceID { get; set; }
        [JsonProperty]
        private int QuarterPreferenceID { get; set; }
        [JsonProperty]
        private DateTime DateAdded { get; set; }
        [JsonProperty]
        private DateTime LastDateModified { get; set; }
        [JsonProperty]
        private int NumberCoreCoursesPerQuarter { get; set; }
        [JsonProperty]
        private int MaxNumberOfQuarters { get; set; }
        [JsonProperty]
        private int CreditsPerQuarter { get; set; }
        [JsonProperty]
        private int Status { get; set; }
        [JsonProperty]
        private string SummerPreference { get; set; }
        [JsonProperty]
        private int EnrollmentType { get; set; }
        #endregion

        #region Getters
        public int getID ()
        {
            return ParameterSetID;
        }
        public int getMajor()
        {
            return MajorID;
        }
        public int getSchool()
        {
            return SchoolID;
        }
        public int getJobType()
        {
            return JobTypeID;
        }
        public int getTimeP()
        {
            return TimePreferenceID;
        }
        public int getQuarterP()
        {
            return QuarterPreferenceID;
        }
        public int getCoreCourse()
        {
            return NumberCoreCoursesPerQuarter;
        }
        public int getMaxQuarters()
        {
            return MaxNumberOfQuarters;
        }
        public int getCreditsPerQuarter()
        {
            return CreditsPerQuarter;
        }
        public string getSummer()
        {
            return SummerPreference;
        }
        public int getEnrollment()
        {
            return EnrollmentType;
        }
        #endregion
    }
}
