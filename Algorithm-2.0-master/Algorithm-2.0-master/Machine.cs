using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System.IO;

namespace Scheduler {
    class Machine {
        #region NOTES
        /*
         * I'm still not sure what exactly a "Machine" is. My supposition is that a Machine is a course complete
         * with their respective days and times. Each job has a correlating day and time representing the fluidity
         * of how courses are scheduled. Such as MWF or M-TH, etc. I'll look into this further... - Andrue
         * 
         * Confirmed that this is indeed to represent an individual course.
         * 
         * I took the liberty of organizing the code into regions of similar code. -Andrue
         */
        #endregion

        #region Variables
        private bool inUse; //Not sure what this is for
        private int year;
        private int quarter;
        private List<Job> jobs; //READ: The courses associated to this machine(?)
        private Job currentJobProcessing;
        private List<DayTime> dateTime; //datetimes from class?
        #endregion

        #region Constructors
        //------------------------------------------------------------------------------
        // 
        // default constructor
        // 
        //------------------------------------------------------------------------------
        public Machine() {
            this.year = 0;
            this.quarter = 0;
            dateTime = new List<DayTime>();
            this.jobs = new List<Job>();
            inUse = false;
            currentJobProcessing = null;
        }

        //------------------------------------------------------------------------------
        // 
        // constructor with data
        // 
        //------------------------------------------------------------------------------
        public Machine(int year, int quarter, List<DayTime> dt, List<Job> jobs) {
            this.year = year;
            this.quarter = quarter;
            dateTime = dt;
            this.jobs = jobs;
            inUse = false;
            currentJobProcessing = null;
        }

        //------------------------------------------------------------------------------
        // 
        // copy constructor; i dont think this is used
        // 
        //------------------------------------------------------------------------------
        public Machine(Machine m) {
            this.year = m.year;
            this.quarter = m.quarter;
            dateTime = m.dateTime;
            jobs = m.jobs;
            inUse = false;
            currentJobProcessing = null;
        }
        #endregion

        #region Needs Documentation
        //------------------------------------------------------------------------------
        // 
        // checks if this job is in the list of the jobs it can process; used by
        // scheduler
        //------------------------------------------------------------------------------
        public bool CanDoJob(Job job) {
            for(int i = 0; i < jobs.Count; i++) {
                if(jobs[i] == job) {
                    return true;
                }
            }
            return false;
        }
        #endregion
        
        #region Dead Code?
        //------------------------------------------------------------------------------
        // 
        // used originally; not used at the moment
        // 
        //------------------------------------------------------------------------------
        public List<int> GetStartTimes() {
            List<int> start = new List<int>();
            for (int i = 0; i < dateTime.Count; i++) {
                start.Add(dateTime[i].GetStartTime());
            }
            return start;
        }

        //------------------------------------------------------------------------------
        // 
        // used originally; not used at the moment
        // 
        //------------------------------------------------------------------------------
        public List<int> GetEndTimes() {
            List<int> end = new List<int>();
            for (int i = 0; i < dateTime.Count; i++) {
                end.Add(dateTime[i].GetStartTime());
            }
            return end;
        }
        #endregion

        #region Status Check (Getters)
        //------------------------------------------------------------------------------
        // 
        // checks if machine is busy
        // 
        //------------------------------------------------------------------------------
        public bool CheckInUse() {
            return inUse;
        }
        //------------------------------------------------------------------------------
        // 
        // returns year
        // 
        //------------------------------------------------------------------------------
        public int GetYear()
        {
            return year;
        }

        //------------------------------------------------------------------------------
        // 
        // returns quarter
        // 
        //------------------------------------------------------------------------------
        public int GetQuarter()
        {
            return quarter;
        }

        //------------------------------------------------------------------------------
        // 
        // returns currentJobProcessing
        // 
        //------------------------------------------------------------------------------
        public Job GetCurrentJobProcessing()
        {
            return currentJobProcessing;
        }
        //------------------------------------------------------------------------------
        // 
        // returns full list list dateTime
        // 
        //------------------------------------------------------------------------------
        public List<DayTime> GetDateTime()
        {
            return dateTime;
        }
        #endregion

        #region Change Status (Setters)
        //------------------------------------------------------------------------------
        // 
        // sets machine in use
        // 
        //------------------------------------------------------------------------------
        public void SetInUse(bool x) {
            inUse = x;
        }

        //------------------------------------------------------------------------------
        // 
        // sets quarter in use
        // 
        //------------------------------------------------------------------------------
        public void SetQuarter(int q) {
            quarter = q;
        }

        //------------------------------------------------------------------------------
        // 
        // sets the year
        // 
        //------------------------------------------------------------------------------
        public void SetYear(int q) {
            year = q;
        }
        //------------------------------------------------------------------------------
        // 
        // sets currentJobProcessing
        // 
        //------------------------------------------------------------------------------
        public void SetCurrentJobProcessing(Job s)
        {
            currentJobProcessing = s;
        }
        #endregion

        #region ADD/REMOVE JOBS
        //------------------------------------------------------------------------------
        // 
        // adds a job to the machine
        // 
        //------------------------------------------------------------------------------
        public void AddJob(Job s) {
            if (!jobs.Contains(s)) jobs.Add(s);
        }

        //------------------------------------------------------------------------------
        // 
        // removes a job; not used; kept for extensions
        // 
        //------------------------------------------------------------------------------
        public void DeleteJob(Job s) {
            if (jobs.Contains(s)) jobs.Remove(s);
        }
        #endregion

        #region Add DayTime
        //------------------------------------------------------------------------------
        // 
        // adds a dateTime to list
        // 
        //------------------------------------------------------------------------------
        public void AddDayTime(DayTime dt) {
            if (!dateTime.Contains(dt)) {
                dateTime.Add(dt);
            }
        }
        #endregion

        #region Display Methods
        //------------------------------------------------------------------------------
        // 
        // prints all the jobs
        // 
        //------------------------------------------------------------------------------
        public void Print() {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Year: " + year);
            Console.WriteLine("Quarter: " + quarter);
            Console.WriteLine("Course ID:");
            for (int i = 0; i < jobs.Count; i++) {
                Job j = jobs[i];
                Console.WriteLine(j.GetID());
            }
            Console.WriteLine("DayTimes:");
            for (int i = 0; i < dateTime.Count; i++) {
                DayTime dt = dateTime[i];
                Console.WriteLine("Day: " + dt.GetDay());
                Console.WriteLine("Start time: " + dt.GetStartTime());
                Console.WriteLine("End time: " + dt.GetEndTime());
            }
            Console.WriteLine("-----------------------------------");
        }

        //------------------------------------------------------------------------------
        // 
        // prints just one job
        // 
        //------------------------------------------------------------------------------

        public void PrintBusyMachine() {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Year: " + year);
            Console.WriteLine("Quarter: " + quarter);
            Console.WriteLine("CourseID:");
            Console.WriteLine(currentJobProcessing.GetID());           
            Console.WriteLine("DayTimes:");
            for (int i = 0; i < dateTime.Count; i++) {
                DayTime dt = dateTime[i];
                Console.WriteLine("Day: " + dt.GetDay());
                Console.WriteLine("Start time: " + dt.GetStartTime());
                Console.WriteLine("End time: " + dt.GetEndTime());
            }
            Console.WriteLine("-----------------------------------");
        }
/*
        public string convertToJson()
        {
            
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();
                writer.WritePropertyName("Year");
                writer.WriteValue(year);
                writer.WritePropertyName("Quarter");
                writer.WriteValue(quarter);
                writer.WritePropertyName("CourseID");
                writer.WriteValue(currentJobProcessing.GetID());
                writer.WritePropertyName("DateTimes");
                writer.WriteStartArray();
                for (int i = dateTime.Count - 1; i >= 0; i--)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("Day");
                    writer.WriteValue(dateTime[i].GetDay());
                    writer.WritePropertyName("StartTime");
                    writer.WriteValue(dateTime[i].GetStartTime());
                    writer.WritePropertyName("EndTime");
                    writer.WriteValue(dateTime[i].GetEndTime());
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                writer.WriteEnd();
            }
            return sw.ToString();

        }
        */
        #endregion

        #region Linear Search for DayTime
        //------------------------------------------------------------------------------
        // 
        // checks if the day time is contained
        // 
        //------------------------------------------------------------------------------
        private bool ContainsDayTime(List<DayTime> times, DayTime dt) {
            for(int i = 0; i < times.Count; i++) {
                DayTime time = times[i];
                if(time == dt) {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Comparison Methods
        //------------------------------------------------------------------------------
        // 
        // equality
        // 
        //------------------------------------------------------------------------------
        public static bool operator ==(Machine thism, Machine right) {
            if (thism.quarter != right.quarter || thism.year != right.year
                || thism.dateTime.Count != right.dateTime.Count) {
                return false;
            }
            for (int i = 0; i < thism.dateTime.Count; i++) {
                if (!thism.ContainsDayTime(thism.dateTime, right.dateTime[i])) {
                    return false;
                }
            }
            return true;
        }

        //------------------------------------------------------------------------------
        // 
        // equality
        // 
        //------------------------------------------------------------------------------
        public static bool operator !=(Machine thism, Machine right) {
            return !(thism == right);
        }

        //------------------------------------------------------------------------------
        // 
        // equality
        // 
        //------------------------------------------------------------------------------
        public override bool Equals(object obj)
        {
            Machine j = obj as Machine;
            if (j == null)
            {
                return false;
            }
            else
            {
                return j == this;
            }
        }
        #endregion
    }
}
