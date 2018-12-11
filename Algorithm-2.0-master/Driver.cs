using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Scheduler {
    class Driver {
        //------------------------------------------------------------------------------
        // the driver hard codes some of the inputs like which major and degree you are
        // looking at. it will take this from the UI later. it prints all the busy
        // machines now. it will pass them to UI later.
        //------------------------------------------------------------------------------
        static void Main(string[] args) {


            //Preferences: Max Quarter and Summer Preference
            Scheduler scheduler = new Scheduler(188);
            //Scheduler scheduler = new Scheduler(20, false);

            #region Starting point
            //Input degree plan

            //Preferences: Starting Point, Major, and School
            //scheduler.MakeStartingPoint("ENGL& 101", "MATH& 141");
            //scheduler.InitDegreePlan(22, 6); //get this from UI later
            #endregion

            #region Make Proposed Schedule
            //make new proposed schedule object
            //List<Machine> schedule =  new List<Machine>();
         
            //Output and Schedule Generation
            //Console.WriteLine("Scheduled following courses:");
            //scheduler.CreateSchedule();
            Console.WriteLine(JsonConvert.SerializeObject(scheduler, Formatting.Indented));
 
            /*print all busy machines*/
            /*
            for (int i = 0; i < schedule.Count; i++) {
                Machine m = schedule[i];
                m.PrintBusyMachine();
            }     
            */
            #endregion

            #region Error Output
            /*print what couldn't be scheduled*/           
            /*
            Console.WriteLine("Unable to schedule following courses:");
            List<Job> unScheduled = scheduler.GetUnscheduledCourses();
            for (int i = 0; i < unScheduled.Count; i++)
            {
                Job j = unScheduled[i];
                Console.WriteLine(j.GetID());
            }
            */
            Console.ReadLine();
            
            #endregion
        }
    }
}
