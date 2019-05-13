using System;
using Newtonsoft.Json;

namespace Scheduler {
    public class Driver {
        //------------------------------------------------------------------------------
        // The driver is used to run the scheduler. Either Method is a valid means to run
        // the scheduler. Should function calls be made to alter the preferences
        // CreateScheduler() would need to be ran again to ensure correct output.
        //------------------------------------------------------------------------------
        static void Main(string[] args) {
            #region Method 1: Manual Input of Preferences
            Scheduler scheduler = new Scheduler(16, false);
            //scheduler.MakeStartingPoint("ENGL& 101", "MATH& 141");
            scheduler.InitDegreePlan(22, 6); //get this from UI later
            scheduler.CreateSchedule();
            #endregion

            //#region Method 2: Automated based off of ParameterSetID
            //Scheduler scheduler = new Scheduler(1);
            //#endregion
            var sch = JsonConvert.SerializeObject(scheduler, Formatting.Indented);
            Console.WriteLine(sch);
            Console.ReadLine();
        }
    }
}
