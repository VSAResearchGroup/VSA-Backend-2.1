using System;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler {
    class Preferences {
        #region NOTES
        /*
         * So it looks like that other than saving the preferences to a hashtable that 
         * this class doesn't really do anything. - Andrue
         * 
         * Took the liberty of separating the code into regions of similar functionality -Andrue
         */
        #endregion

        #region VARIABLES
        //Hashtable preferences;
        private string JsonString;
        private List<ParameterSet> prefs;
        private List<Job> priors;
        bool summerIntent;

        private class CourseNumbers
        {
            [JsonProperty]
            private string CourseNumber { get; set; }

            public string returnCourse()
            {
                return CourseNumber;
            }
        }


        private DBConnection dbHit;
        #endregion

        #region Constructors
        //------------------------------------------------------------------------------
        // 
        // default constructor
        // 
        //------------------------------------------------------------------------------
        public Preferences() {
            JsonString = "Data Source = 65.175.68.34; Initial Catalog = vsaDev; Persist Security Info = True; User ID = sa; Password = kD$wg&OUrhfC6AMMq6q5Xh"j;
        }

        public Preferences(string JsonInput)
        {
            JsonString = JsonInput;
            Deserialize();
        }

        public Preferences(int parameterID)
        {
            dbHit = new DBConnection();
            JsonString = "Data Source = 65.175.68.34; Initial Catalog = vsaDev; Persist Security Info = True; User ID = sa; Password = kD$wg&OUrhfC6AMMq6q5Xh"j;
            JsonString = dbHit.ExecuteToString("select * from ParameterSet where ID =" + parameterID + "for JSON AUTO;");
            Deserialize();
            SetPriors();
            determineSummer();
        }
        #endregion

        #region JsonDeserializer
        private void Deserialize()
        {
            prefs = new List<ParameterSet>();
            prefs = JsonConvert.DeserializeObject<List<ParameterSet>>(JsonString, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
            //Console.WriteLine("ID: " + prefs[0].getID());
            //Console.WriteLine("Major " + prefs[0].getMajor());
            //Console.WriteLine("School " + prefs[0].getSchool());
            //Console.WriteLine("Time " + prefs[0].getTimeP());
            //Console.WriteLine("Quarter " + prefs[0].getQuarterP());
            //Console.WriteLine("Completed " + prefs[0].getCompleted());
            //Console.WriteLine("Placement " + prefs[0].getPlacement());
            //Console.WriteLine("Budget " + prefs[0].getBudget());
            //Console.WriteLine("Summer? " + prefs[0].getSummer());
            //Console.WriteLine("Enrollment " + prefs[0].getEnrollment());
            //Console.WriteLine(JsonString);
        }

        public List<Job> getPriors()
        {
            return priors;
        }


        private void SetPriors()
        {
            //Console.WriteLine(prefs[0].getCompleted());
            priors = new List<Job>();
            addTopriors(prefs[0].getCompleted());
            addTopriors(prefs[0].getPlacement());
            //for (int i = 0; i < priors.Count; i++)
            //{
            //    Console.WriteLine("ID: " + priors[i].GetID());
            //}
        }

        private void addTopriors(string passed)
        {
            List<CourseNumbers> courses = JsonConvert.DeserializeObject<List<CourseNumbers>>(passed);
            for (int i = 0; i < courses.Count; i++)
            {
                //Console.WriteLine(courses[0].returnCourse());
                DataTable courseID = dbHit.ExecuteToDT("select CourseID from Course where CourseNumber = '" + courses[i].returnCourse() + "' ;");

                Job tempJob = new Job((int)courseID.Rows[0].ItemArray[0]);
                tempJob.SetScheduled(true);
                priors.Add(tempJob);
                //Console.WriteLine(tempJob.GetID());
            }
        }
        public int getMajor()
        {
            return prefs[0].getMajor();
        }

        public int getSchool()
        {
            return prefs[0].getSchool();
        }

        public int getQuarters()
        {
            return prefs[0].getBudget();
        }

        public bool getSummer()
        {
            return summerIntent;
        }
        private void determineSummer()
        {
            char test = prefs[0].getSummer()[0];
            Console.WriteLine(test);
            if (test == 'Y' || test == 'y')
            {
                summerIntent = true;
            }
            Console.WriteLine(summerIntent);
        }
        
        #endregion
    }
}
