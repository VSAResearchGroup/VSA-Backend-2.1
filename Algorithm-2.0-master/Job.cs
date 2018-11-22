using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler {
    class Job {
        #region NOTES
        /*
         * This appears to be how courses are represented. Each course has an ID,
         * a boolean for if the course was scheduled, a boolean for if the course has
         * all the prerequisites of at least one prerequisite group scheduled, which quarter
         * the prerequisite was scheduled, and which year it was scheduled. -Andrue
         * 
         * I took the liberty of reorganizing the code into regions of like code, all the getter methods in one location,
         * all the setter methods in another location, etc. -Andrue
         */
        #endregion

        #region Class Variables
        private int id;
        private bool scheduled;
        private bool prerequisitesScheduled;
        private int quarterScheduled;
        private int yearScheduled;
        //need to implement int credits for the course for preferences purposes
        //need to implement boolean if its a major course or not for preferences purposes
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------
        // 
        // constructor
        // 
        //------------------------------------------------------------------------------
        public Job(int id) {
            this.id = id;
            scheduled = false;
            quarterScheduled = -1;
            yearScheduled = -1;
            prerequisitesScheduled = false;
        }
        #endregion

        #region Getters
        //------------------------------------------------------------------------------
        // 
        // boolean for scheduled or not
        // 
        //------------------------------------------------------------------------------
        public bool GetScheduled() {
            return scheduled;
        }

        //------------------------------------------------------------------------------
        // 
        // boolean for if the prerequisites are scheduled
        // 
        //------------------------------------------------------------------------------
        public bool GetPrerequisitesScheduled() {
            return prerequisitesScheduled;
        }

        //------------------------------------------------------------------------------
        // 
        // getter for wuarter scheduled
        // 
        //------------------------------------------------------------------------------
        public int GetQuarterScheduled()
        {
            return quarterScheduled;
        }
        //------------------------------------------------------------------------------
        // 
        // getter for year scheduled
        // 
        //------------------------------------------------------------------------------
        public int GetYearScheduled()
        {
            return yearScheduled;
        }
        //------------------------------------------------------------------------------
        // 
        // getter for id of course; corresponds to what it is in DB
        // 
        //------------------------------------------------------------------------------
        public int GetID()
        {
            return id;
        }
        #endregion

        #region Setters
        //------------------------------------------------------------------------------
        // 
        // setter for prerequisites
        // 
        //------------------------------------------------------------------------------
        public void SetPrerequisitesScheduled(bool b) {
            prerequisitesScheduled = b;
        }

        //------------------------------------------------------------------------------
        // 
        // seter for being scheduled
        // 
        //------------------------------------------------------------------------------
        public void SetScheduled(bool s) {
            scheduled = s;
        }

        //------------------------------------------------------------------------------
        // 
        // setter for quarter scheduled
        // 
        //------------------------------------------------------------------------------
        public void SetQuarterScheduled(int x) {
            quarterScheduled = x;
        }
        //------------------------------------------------------------------------------
        // 
        // setter for year scheduled
        // 
        //------------------------------------------------------------------------------
        public void SetYearScheduled(int x)
        {
            yearScheduled = x;
        }
        #endregion

        #region Comparison Operator
        //------------------------------------------------------------------------------
        // 
        // equality
        // 
        //------------------------------------------------------------------------------
        public static bool operator ==(Job thisj, Job right) {
            if(object.ReferenceEquals(thisj, null) || object.ReferenceEquals(right, null)) {
                return false;
            }
            if(object.ReferenceEquals(thisj, right)) {
                return true;
            }
            return thisj.Equals(right);
        }

        //------------------------------------------------------------------------------
        // 
        // equality
        // 
        //------------------------------------------------------------------------------
        public static bool operator !=(Job thisj, Job right) {
            return !(thisj == right);
        }

        //------------------------------------------------------------------------------
        // 
        // equality
        // 
        //------------------------------------------------------------------------------
        public override bool Equals(object obj) {
            Job j = obj as Job;
            return this.id == j.id;
        }
        #endregion
    }
}
