using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public class ParameterSet
    {
        [JsonProperty]
        private int ID { get; set; }
        [JsonProperty]
        private int MajorID { get; set; }
        [JsonProperty]
        private int SchoolID { get; set; }
        [JsonProperty]
        private int BudgetID { get; set; }
        [JsonProperty]
        private int TimePreferenceID { get; set; }
        [JsonProperty]
        private int QuarterPreferenceID { get; set; }
        [JsonProperty]
        private string CompletedCourses { get; set; }
        [JsonProperty]
        private string PlacementCourses { get; set; }
        [JsonProperty]
        private DateTime DateAdded { get; set; }
        [JsonProperty]
        private DateTime LastDateModified { get; set; }
        [JsonProperty]
        private int Status { get; set; }
        [JsonProperty]
        private string SummerPreference { get; set; }
        [JsonProperty]
        private string EnrollmentType { get; set; }

        public int getID ()
        {
            return ID;
        }
        public int getMajor()
        {
            return MajorID;
        }
        public int getSchool()
        {
            return SchoolID;
        }
        public int getBudget()
        {
            return BudgetID;
        }
        public int getTimeP()
        {
            return TimePreferenceID;
        }
        public int getQuarterP()
        {
            return QuarterPreferenceID;
        }
        public string getCompleted()
        {
            return CompletedCourses;
        }
        public string getPlacement()
        {
            return PlacementCourses;
        }
        public string getSummer()
        {
            return SummerPreference;
        }
        public string getEnrollment()
        {
            return EnrollmentType;
        }
        
    }
}
