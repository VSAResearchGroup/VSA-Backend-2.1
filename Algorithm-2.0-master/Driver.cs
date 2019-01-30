using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler {
    public class Driver {
        //------------------------------------------------------------------------------
        // the driver hard codes some of the inputs like which major and degree you are
        // looking at. it will take this from the UI later. it prints all the busy
        // machines now. it will pass them to UI later.
        //------------------------------------------------------------------------------
        public static void Main(string[] args)
        {
            MakeSchedule(12, false, 12, 22, 6);
        }

        public static List<Machine> MakeSchedule(int quartersDeclared, bool summerIntent, int parameterSet, int majorId, int schoolId)
        {
            Scheduler scheduler = new Scheduler(quartersDeclared, summerIntent, parameterSet);

            #region Starting point
            //Input degree plan
            //scheduler.MakeStartingPoint("nothing yet");
            scheduler.InitDegreePlan(majorId, schoolId); //get this from UI later
            #endregion

            #region Make Proposed Schedule
            //make new proposed schedule object
            List<Machine> schedule = new List<Machine>();

            //Output and Schedule Generation
            Console.WriteLine("Scheduled following courses:");
            schedule = scheduler.CreateSchedule();

            /*print all busy machines*/
            for (int i = 0; i < schedule.Count; i++)
            {
                Machine m = schedule[i];
                m.PrintBusyMachine();
            }
            #endregion

            #region Error Output
            /*print what couldn't be scheduled*/
            Console.WriteLine("Unable to schedule following courses:");
            List<Job> unScheduled = scheduler.GetUnscheduledCourses();
            for (int i = 0; i < unScheduled.Count; i++)
            {
                Job j = unScheduled[i];
                Console.WriteLine(j.GetID());
            }
            #endregion

            return schedule;
        }
    }
}
