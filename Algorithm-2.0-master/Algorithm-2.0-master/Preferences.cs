using System;
using System.Collections;

using System.Collections.Generic;
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
        Hashtable preferences;
        #endregion

        #region Constructors
        //------------------------------------------------------------------------------
        // 
        // default constructor
        // 
        //------------------------------------------------------------------------------
        public Preferences() {
            preferences = new Hashtable();
        }
        #endregion

        #region Adjusters/Setters
        //------------------------------------------------------------------------------
        // 
        // extendable to add more preferences
        // 
        //------------------------------------------------------------------------------
        public void AddPreference(String name, Object a) {
            if(!preferences.ContainsKey(name)) preferences.Add(name, a);
        }

        //------------------------------------------------------------------------------
        // 
        // removes preferences
        // 
        //------------------------------------------------------------------------------
        public void DeletePreference(String name) {
            if (preferences.ContainsKey(name)) preferences.Remove(name);
        }
        #endregion

        #region Getters
        //------------------------------------------------------------------------------
        // 
        // checks if a preference exists; not used at the moment
        // 
        //------------------------------------------------------------------------------
        public bool Exists(String name) {
            return preferences.ContainsKey(name);
        }

        //------------------------------------------------------------------------------
        // 
        // returns a preference
        // 
        //------------------------------------------------------------------------------
        public Object GetPreference(String name) {
            return preferences[name];
        }
        #endregion
    }
}
