using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;

namespace Scheduler {
    public class Scheduler {
        #region NOTES
        /* This part of the algorithm, understandably, has a lot going on inside of it. THough functional, 
         * I believe that some improvements could be made. For example the SQL stuff region should probably 
         * be it's own class entirely. -Andrue
         * 
         * The SQL queries that Polina utilized do not use the JSON data object and instead uses DataTable -Andrue
         * 
         * I've taken the liberty of rearranging the code into sections of similar functionality and encapsulated
         * them into regions - Andrue
         */
        #endregion

        #region Class Variables
        [JsonIgnore]
        private DBConnection DBPlugin;  //Declare the SQL connection to Database
        [JsonIgnore]
        private CourseNetwork network;   //A network of Courses and their respective prerequisite chains
        [JsonIgnore]
        private List<MachineNode> machineNodes; //Each node is one Quarter
        [JsonIgnore]
        private DegreePlan myPlan;       //pull from DB *STORED?* <===== Need to investigate
        [JsonProperty]
        private List<Machine> finalPlan; //output schedule
        [JsonIgnore]
        private Preferences preferences; //obtainable from the database or by manual entry
        [JsonIgnore]
        private List<Job> completedPrior;
        [JsonProperty]
        private List<Job> unableToSchedule;//list of courses that didn't fit into the schedule
        [JsonIgnore]
        private int quarters = 0; //Preference
        [JsonIgnore]
        private bool attendSummer = false; //Preference here for feedback if needed
        [JsonIgnore]
        private int yearlength = 0; //either 3 quarters or 4 quarters depending on Summer preference
        [JsonIgnore]
        private int years; //derived from dividing the total amount of quarters by the yearlength
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------
        // 
        // Constructors
        // 
        //------------------------------------------------------------------------------
        public Scheduler()
        {
            SetUp(8, true, -1);
        }

        public Scheduler(int quartersDeclared, bool summerIntent, bool preferShortest=true)
        {
            SetUp(quartersDeclared, summerIntent, -1);
        }

        public Scheduler(int paramID, bool preferShortest=true)
        {
            SetUp(16, false, paramID);
            MakeStartingPoint();
            InitDegreePlan();
            CreateSchedule(preferShortest);
        }
        #endregion

        #region Setup
        //------------------------------------------------------------------------------
        // Does the setup of the variables and runs functions common to the constructors
        //
        // PARAMID >= 0: Queries the database for preferences and ignores quartersDeclared
        // and summerIntent. MakeStartingPoint and InitDegreePlan can be automated through
        // input from the database query
        //
        // PARAMID < 0: Runs default constructor for preferences signifying methods must be 
        // called to run the algorithm. Setup of schedule structure is dependent on 
        // quartersDeclared and summerIntent. MakeStartingPoint(optional) and 
        // InitDegreePlan(mandatory) would then need to be invoked to run algorithm
        //------------------------------------------------------------------------------
        private void SetUp(int quartersDeclared, bool summerIntent, int paramID)
        {
            DBPlugin = new DBConnection();
            machineNodes = new List<MachineNode>(); //Quarters
            finalPlan = new List<Machine>(); //Courses
            completedPrior = new List<Job>();
            unableToSchedule = new List<Job>();

            if (paramID >= 0)
            {
                preferences = new Preferences(paramID);
                DeterminePlanLength(preferences.getQuarters(), preferences.getSummer());
            }
            else
            {
                preferences = new Preferences();
                DeterminePlanLength(quartersDeclared, summerIntent);
            }

            InitializeMachineNodes();
            InitMachines();
            normalizeMachines(); //Disposable after live data
            InitNetwork();
        }
 
        //------------------------------------------------------------------------------ 
        // initiates Andrue Cashman's network
        //------------------------------------------------------------------------------
        private void InitNetwork()
        {
            //VARIABLES
            string rawpreqs = DBPlugin.ExecuteToString("Select CourseID, GroupID, PrerequisiteID, PrerequisiteCourseID from Prerequisite for JSON AUTO");
            string rawcourses = DBPlugin.ExecuteToString("select CourseID from Course for JSON AUTO;");
            //NETWORK BUILD
            network = new CourseNetwork(rawcourses, rawpreqs);
            network.BuildNetwork();
         }

        //------------------------------------------------------------------------------
        // Determines length of a school year based on the maximum number of quarters
        // and the intention to attend summer courses
        //------------------------------------------------------------------------------
        private void DeterminePlanLength(int quartersDeclared, bool summerIntent)
        {
            if (summerIntent)
            {
                attendSummer = true;
                quarters = quartersDeclared;
                years = quarters / 4;
                yearlength = 4;
            }
            else
            {
                attendSummer = false;
                quarters = quartersDeclared;
                years = quarters / 3;
                yearlength = 3;
            }
        }

        //------------------------------------------------------------------------------
        // creates a list of jobs that are ignored by the algorithm.
        // PRIVATE METHOD: starting point is determined by database query (table: ParameterSet)
        // PUBLIC METHOD: Starting Point determined by strings passed to function
        //------------------------------------------------------------------------------
        private void MakeStartingPoint()
        {
 
            completedPrior = preferences.getPriors();

        }

        public void MakeStartingPoint(string english, string math)
        {
            DataTable mathStart = DBPlugin.ExecuteToDT("select CourseID from Course where CourseNumber = '" + math + "' ;");
            DataTable EnglStart = DBPlugin.ExecuteToDT("select CourseID from Course where CourseNumber = '" + english + "' ;");
           
            Job tempJob = new Job((int)mathStart.Rows[0].ItemArray[0]);
            tempJob.SetScheduled(true);
            completedPrior.Add(tempJob);

            tempJob = new Job((int)EnglStart.Rows[0].ItemArray[0]);
            tempJob.SetScheduled(true);
            completedPrior.Add(tempJob);
        }

        //------------------------------------------------------------------------------
        // Creates machineNodes which are representative of the quarters given through 
        // preferences. For example: if 8 quarters are declared, 8 machineNodes are 
        // created. 
        //------------------------------------------------------------------------------
        private void InitializeMachineNodes()
        {
            if (quarters < yearlength)
            {
                for (int i = 1; i <= quarters; i++)
                {
                    MachineNode m = new MachineNode(0, i);
                    machineNodes.Add(m);
                }
            }
            else
            {
                int quarterCount = 0;
                while (quarterCount < quarters)
                {
                    for (int i = 0; i <= years; i++)
                    {
                        for (int j = 1; j <= yearlength; j++)
                        {
                            MachineNode m = new MachineNode(i, j);
                            machineNodes.Add(m);
                            quarterCount++;
                            if (quarterCount >= quarters)
                                break;
                        }
                    }
                }
            }
        }

        //------------------------------------------------------------------------------
        // WARNING!!! - This function can overwrite information. PLEASE READ
        //
        // This makes the list of classes available over successive years. Current data
        // being used is for the duration of one year, therefore this function sets up
        // the machineNodes in such a way they work off the assumption that classes
        // will be held at the same time, on the same days, year after year.
        //------------------------------------------------------------------------------
        void normalizeMachines()
        {   //transfer all the same classes to the set of machine nodes
            if (quarters >= yearlength)
            {
                for (int i = yearlength; i < quarters; i++)
                {
                    MachineNode oldMn = machineNodes[i % yearlength];
                    MachineNode newMn = machineNodes[i];
                    for (int j = 0; j < oldMn.GetMachines().Count; j++)
                    {
                        Machine oldMachine = oldMn.GetMachines()[j];
                        Machine newMachine = new Machine(oldMachine);
                        newMachine.SetYear(i / yearlength);
                        newMn.AddMachine(newMachine);
                    }
                }
                return;
            }
            else
            {
                return;
            }
  
        }

        //------------------------------------------------------------------------------
        // Queries the database for all the courses needed to complete a specific major
        // at a specific school.
        //
        // PRIVATE METHOD: Major and School are indicated by Preferences. (table: ParameterSet)
        // PUBLIC METHOD: Major and School are indicated by passed parameters
        //------------------------------------------------------------------------------
        public void InitDegreePlan(int majorID, int schoolID)
        {
            string query = "select CourseID from AdmissionRequiredCourses where MajorID ="
                            + majorID + " and SchoolID = " + schoolID + " order by CourseID ASC ";
            planBuilder(DBPlugin.ExecuteToDT(query));

        }

        private void InitDegreePlan()
        {
            string query = "select CourseID from AdmissionRequiredCourses where MajorID ="
                + preferences.getMajor() + " and SchoolID = " + preferences.getSchool() + " order by CourseID ASC";
            planBuilder(DBPlugin.ExecuteToDT(query));
        }

        //------------------------------------------------------------------------------
        // HELPER FUNCTION FOR InitDegreePlan
        // Adds the courses from the query to the list of courses that need to be scheduled
        //------------------------------------------------------------------------------
        private void planBuilder(DataTable dt)
        {
            List<Job> courseNums = new List<Job>();
            foreach (DataRow row in dt.Rows)
            {
                Job job = new Job((int)row.ItemArray[0]);
                courseNums.Add(job);
            }
            myPlan = new DegreePlan(courseNums);
        }

        //------------------------------------------------------------------------------
        // WARNING!! --Please Read SPECIAL NOTE below--
        // Runs a query which pulls all the courses that exist in CourseTime. Conceptually,
        // this creates a list of every class that has been offered. Details like the 
        // Course ID, the time the class starts, the time the class ends, days offered,
        // quarter offered, and the section ID or collected to their respective 
        // counterparts. This provides a means to reduce the amount of objects being handled
        // and can allow implmentation of day-specific and time-specific preferences.
        // 
        // Various checks prevent duplication of machines.
        //
        // SPECIAL NOTE: If we have the means to, and are required to do so, of 
        //               implementing different course offerings on a yearly basis
        //               this is where we would need to change it. DOING SO WOULD MEAN
        //               THAT THE FUNCTION, NORMALIZEMACHINES(), WOULD BE INCOMPATIBLE
        //               WITH THIS ALGORITHM.
        //------------------------------------------------------------------------------
        private void InitMachines()
        {
            string query = "select CourseID, StartTimeID, EndTimeID, DayID, QuarterID, SectionID from CourseTime order by CourseID ASC;";
            DataTable dt = DBPlugin.ExecuteToDT(query);
            int dt_size = dt.Rows.Count - 1;
            DataRow dr = dt.Rows[dt_size];

            //Temporary Machine Variables
            Machine dummyMachine = new Machine();
            DayTime dummyDayTime = new DayTime();
            int course = 0;
            int start = 0;
            int end = 0;
            int day = 0;
            int quarter = 0;
            int section = 0;
            int currentCourse = (int)dr.ItemArray[0];  //USED FOR PEAKING THE NEXT ROW
            int currentQuarter = (int)dr.ItemArray[4]; //USED FOR PEAKING THE NEXT ROW
            int currentSection = (int)dr.ItemArray[5]; //USED FOR PEAKING THE NEXT ROW

            //Treats the information gained from the query like a FILO object
            while (dt_size >= 0)
            {
                dr = dt.Rows[dt_size];
                //check for null values
                if (dr.ItemArray[0] == DBNull.Value || dr.ItemArray[1] == DBNull.Value ||
                    dr.ItemArray[2] == DBNull.Value || dr.ItemArray[3] == DBNull.Value ||
                    dr.ItemArray[4] == DBNull.Value || dr.ItemArray[5] == DBNull.Value)
                {
                    dt_size--; //IF any portion is null, then the row is discarded entirely.
                    continue;
                }
                //going to have to do the same with year probably; Andrue Note: Most likely the case
                course = (int)dr.ItemArray[0];
                start = (int)dr.ItemArray[1];
                end = (int)dr.ItemArray[2];
                day = (int)dr.ItemArray[3];
                quarter = (int)dr.ItemArray[4];
                section = (int)dr.ItemArray[5];

                //same course but different section OR different quarter is a different machine
                //different course is a different machine 
                if ((currentCourse == course && (currentSection != section || currentQuarter != quarter)) || (currentCourse != course))
                {
                    dummyMachine = new Machine(); //creates a new machine to be used
                    currentCourse = (int)dr.ItemArray[0];
                    currentQuarter = (int)dr.ItemArray[4];
                    currentSection = (int)dr.ItemArray[5];
                }

                dummyDayTime = new DayTime();
                dummyDayTime.SetDayTime(day, start, end);
                dummyMachine.AddDayTime(dummyDayTime);
                dummyMachine.SetQuarter(quarter);

                //we add a new machine when we peek to the next row and see
                //(different course) OR (same course and (different section OR dif qtr))
                //Andrue Note: Maybe isolate these arguments into helper functions for ease-of-use?
                //if (itself(?)) OR (not same course) OR (IS course but NOT SAME Section OR Quarter)
                int next = dt_size - 1;
                if (dt_size == 0 || ((int)dt.Rows[next].ItemArray[0] != currentCourse ||
                    ((int)dt.Rows[next].ItemArray[0] == currentCourse &&
                    ((int)dt.Rows[next].ItemArray[5] != currentSection)
                    || (int)dt.Rows[next].ItemArray[4] != currentQuarter)))
                {
                    addMachine(dummyMachine, course);
                }
                dt_size--;
            }
        }

        //------------------------------------------------------------------------------
        // HELPER FUNCTION FOR INITMACHINES()
        //
        // Adds a machine to the machine list for offered courses by first doing a search
        // amongst the machineNodes if the Course already exists there and acts 
        // accordingly.
        //------------------------------------------------------------------------------
        void addMachine(Machine dummyMachine, int course)
        {
            dummyMachine.AddJob(new Job(course)); //adds job
            for (int i = 0; i < machineNodes.Count; i++)
            {
                MachineNode mn = machineNodes[i];
                List<Machine> machines = mn.GetMachines();
                if (machines.Count > 0)
                {
                    for (int j = 0; j < machines.Count; j++)
                    {
                        Machine m = machines[j];
                        if (m == dummyMachine)
                        { //found the machine, just add job
                            m.AddJob(new Job(course));
                            break;
                        }
                        else if (dummyMachine.GetYear().Equals(mn.GetYear()) && dummyMachine.GetQuarter().Equals(mn.GetQuarter()))
                        { //machine does not exist, add it in
                             machines.Add(dummyMachine);
                             break;
                        }
                    }
                }
                else if (dummyMachine.GetYear().Equals(mn.GetYear()) && dummyMachine.GetQuarter().Equals(mn.GetQuarter()))
                {
                     machines.Add(dummyMachine);
                     break;
                }
                else //in the instance that machines == 0 and either year or quarter were different 
                {
                    //NOTE: This isn't so much an error as a bookkeeping check. Because CourseTime contains only 1 year
                    //      machines dated beyond the first year throw this error. So this is a database issue.
                    /*
                     Console.WriteLine("Dummy Machine Year: " + dummyMachine.GetYear());
                     Console.WriteLine("Dummy Machine Quarter: " + dummyMachine.GetQuarter());
                     Console.WriteLine("Dummy Course ID: " + course);
                     Console.WriteLine("mn Year: " + mn.GetYear());
                     Console.WriteLine("mn Quarter: " + mn.GetQuarter());
                     Console.WriteLine('\n');
                     */
                }
            }
        } 
        #endregion

        #region Scheduling Algorithm
        //------------------------------------------------------------------------------
        // Creates a Schedule based on the required courses that need to be scheduled
        // (given by the function, InitDegreePlan().
        //
        // SPECIAL NOTE: This is most likely the entry point for where we would need
        //               to implement electives or optional courses should we need to
        //               implement those. 
        //
        // Afterwards, this course returns the resulting schedule.
        //------------------------------------------------------------------------------
        public List<Machine> CreateSchedule(bool preferShortest)
        {
            List<Job> majorCourses = myPlan.GetList(0); //LIST OF REQUIRED COURSES
            for (int i = 0; i < majorCourses.Count; i++)
            {
                Job job = majorCourses[i]; //GET NEXT CLASS IN LIST
                ScheduleCourse(job, preferShortest); //FUNCTION CALL TO SCHEDULE LIST
            }
            finalPlan = GetBusyMachines(); //SUGGEST BETTER NAMING CONVENTION?//
            //return proposed schedule
            return finalPlan;
        }
    
        //------------------------------------------------------------------------------
        // Uses recursive calls to schedule prerequisites of the passed job before 
        // scheduling the job itself.
        //
        // If for some reason a course cannot be scheduled (due to scheduling conflicts)
        // then that course is added a supplmentary list of unscheduled coursees.
        //------------------------------------------------------------------------------
        private void ScheduleCourse(Job job, bool preferShortest)
        {
            #region ENDCASE
            if (IsScheduled(job)) //CHECK IF CLASS IS SCHEDULED
            {
                return; //IF SCHEDULED; FINISH
            }
            #endregion

            #region RECURSIVE SCHEDULING CALL
            int num = job.GetID(); //ID OF COURSE BEING SCHEDULED
            List<CourseNode> groups = network.FindShortPath(num);//FINDS PREREQUISITE COURSES FOR THE GIVEN COURSE
            if (PrereqsExist(groups) && !job.GetPrerequisitesScheduled())
            {   //if j does not have prerequisites (OR its prerequisites have been scheduled) schedule j  
                //schedule j's prerequisites by getting shortest group and whatnot
                int selectedGroup;
                if (preferShortest)
                {
                   selectedGroup = GetShortestGroup(groups); //FIND GROUP WITH LEAST PREREQUISITES
                }
                else
                {
                    selectedGroup = GetAnyGroup(groups); //Find any group is OK
                }
                
                List<CourseNode> group = groups[selectedGroup].prereqs; //GET LIST OF PREREQUISITES 

                List<Job> jobsToBeScheduled = new List<Job>();
                for (int j = 0; j < group.Count; j++)  //TRANSFER PREREQUSITE LIST INTO MORE JOBS
                {
                    Job myJob = new Job(group[j].PrerequisiteCourseID);
                    jobsToBeScheduled.Add(myJob);
                }//now we have a list full of jobs to be scheduled

                for (int k = 0; k < jobsToBeScheduled.Count; k++) //SCHEDULE THE PREREQUISITES
                { //schedule them all here
                    ScheduleCourse(jobsToBeScheduled[k], preferShortest);
                }//now they are scheduled
                job.SetPrerequisitesScheduled(true);
            }
            #endregion

            #region SCHEDULE COURSE
            PutCourseOnMachine(job, groups); //SCHEDULE COURSE
            #endregion

            #region Error Messages
            if (!job.GetScheduled())
            { //figure out what to do if it wasn't able to be scheduled, make this a function later
                unableToSchedule.Add(job);
            }
            #endregion
        }

        //------------------------------------------------------------------------------
        // Puts a course into the schedule by first checking the course's most
        // immediate prerequisite that has been scheduled and starting from the next
        // nearest schedulable machineNode. From the starting point the course is then
        // scheduled according to preferences.
        // 
        // SPECIAL NOTE: As Polina writes below, this is indeed a perfect function to
        //               implement several preferences invloved with courses and scheduling.
        //               Namely, Courses per quarter, day preferences, and timeOfDay preferences.
        //               I took the liberty of labeling the best spots to place these.
        //               Additionally, should Machines ever have a CoreCourse, or other attributes
        //               (Diversity, Humanities, etc.) checks could be placed in this function
        //               to schedule those courses.
        // 
        // does the actual action of putting a course on a machine; this will be the hub
        // for implementing preferences, not all are implemented at the moment; also,
        // right now unscheduled courses are simply going into a list but if you were to 
        // do something else, this function is where it would originate
        //------------------------------------------------------------------------------
        private void PutCourseOnMachine(Job j, List<CourseNode> groups)
        {
            #region variables
            //get most recent prereq
            int mostRecentPrereqYear = 0;
            int mostRecentPrereqQuarter = 1;
            int start = 0;
            #endregion

            #region Prerequisite Handler
            //if no prereqs then schedule at any time
            if (PrereqsExist(groups)) //CHECKS FOR NULL
            { //this is if there are prereqs
                int[] yq = GetMostRecentPrereq(groups); 

                mostRecentPrereqYear = yq[0]; 
                mostRecentPrereqQuarter = yq[1]; 

                //ERROR CHECK
                if (mostRecentPrereqQuarter == -1 || mostRecentPrereqYear == -1)
                { //has prerequisites but they weren't able to be scheduled
                    unableToSchedule.Add(j);
                    return;
                }

                //schedule 1 or more quarters after, mind the year <--(?)
                //schedule on nearest available machine
                //start i at whatever quarter you calculate, not simply zero
                start = (mostRecentPrereqYear * 4 + mostRecentPrereqQuarter - 1) + 1;
            }
            #endregion


            for (int i = start; i < machineNodes.Count; i++)
            {
                MachineNode mn = machineNodes[i];
                //if machine node exeeds preferences continue to next node
                if (mn.GetClassesScheduled() > 3) //<<---------------------------- TOTAL COURSES PER QUARTER PREFERENCE(?)
                {
                    continue;
                }
                List<Machine> machines = mn.GetMachines();

                for (int k = 0; k < machines.Count; k++)
                {
                    //<<----------------------------INSERT DAY/TIME PREFERENCE AND CHECK AGAINST IT
                    Machine m = machines[k];
                    if (m.CanDoJob(j) && !m.CheckInUse())
                    { //if not in use and it can do the job
                        if (Overlap(j, m, mn))
                        { //can't schedule it if the times overlap even if machine found
                            continue;
                        }
                        m.SetCurrentJobProcessing(j);
                        m.SetInUse(true);
                        j.SetScheduled(true);
                        j.SetQuarterScheduled(m.GetQuarter());
                        j.SetYearScheduled(m.GetYear());
                        mn.AddClassesScheduled(1);
                        finalPlan.Add(m);
                        return;
                    }
                }
            }
        }
        #endregion

        #region Results
        //------------------------------------------------------------------------------
        // This returns a list of the courses that have been scheduled
        //------------------------------------------------------------------------------
        public List<Machine> GetBusyMachines()
        {
            List<Machine> busy = new List<Machine>();
            for (int i = 0; i < machineNodes.Count; i++)
            {
                //Console.WriteLine(machineNodes.Count);
                //Console.WriteLine(machineNodes[i].GetYear() + " " + machineNodes[i].GetQuarter());
                List<Machine> machines = machineNodes[i].GetAllScheduledMachines();
                for (int j = 0; j < machines.Count; j++)
                {
                    busy.Add(machines[j]);
                }
            }
            return busy;
        }

        //------------------------------------------------------------------------------
        // This returns a list of courses that were not scheduled.
        //------------------------------------------------------------------------------
        public List<Job> GetUnscheduledCourses()
        {
            return unableToSchedule;
        }
        #endregion

        #region Helper Functions
        //------------------------------------------------------------------------------
        // checks if a course is already scheduled. Because courses are returned as 
        // numbers from Cashman network and not Job type, we can't check for the 
        // instance, we have to find it
        //------------------------------------------------------------------------------
        private bool IsScheduled(Job j)
        {
            if (completedPrior != null)
            {
                if (completedPrior.Count > 0)
                {
                    for (int i = 0; i < completedPrior.Count; i++)
                    {
                        if (j.GetID() == completedPrior[i].GetID())
                        {
                            return true;
                        }
                    }
                }
            }

            for (int i = 0; i < finalPlan.Count; i++)
            {
                Machine m = finalPlan[i];
                if (m.GetCurrentJobProcessing().GetID() == j.GetID())
                {
                    return true;
                }
            }
            return false;
        }

        //------------------------------------------------------------------------------
        // checks if prerequisite exists; this function can be eliminated, I just didn't
        // quite understand why cashman network had so many lists of lists.
        // 
        // THIS IS A GOOD CHECK AGAINST UNNECCESSARY WORK 
        //------------------------------------------------------------------------------
        private bool PrereqsExist(List<CourseNode> groups)
        {
            if (groups != null)
            {
                for (int i = 0; i < groups.Count; i++)
                {
                    if (groups[i].prereqs != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //------------------------------------------------------------------------------
        // AN ERROR CHECKING METHOD TO PREVENT CLASSES FROM HAPPENING DURING THE SAME TIME
        // check to see if a job overlaps with another job's times in a single 
        // MachineNode; can't be in 2 places at once you know?
        //------------------------------------------------------------------------------
        private bool Overlap(Job j, Machine goal, MachineNode mn)
        {
            bool flag = false; //Defaulted to continue

            //need list of all the start and end times from goal
            List<DayTime> dt = goal.GetDateTime();
            List<Machine> myMachines = mn.GetAllScheduledMachines(); //Look for Scheduled Courses in All Quarters

            for (int i = 0; i < myMachines.Count; i++)
            {
                Machine m = myMachines[i];
                List<DayTime> tempDT = m.GetDateTime();

                //Each class is available for the same amount of days
                if (dt.Count == tempDT.Count)
                {
                    for (int k = 0; k < dt.Count; k++)
                    {
                        //Checks to see if the start time or end time exists between the start and end of a scheduled course
                        if ((dt[k].GetStartTime() >= tempDT[k].GetStartTime() && dt[k].GetStartTime() <= tempDT[k].GetEndTime()) ||
                        (dt[k].GetEndTime() >= tempDT[k].GetStartTime() && dt[k].GetEndTime() <= tempDT[k].GetEndTime()))
                        {
                            flag = true;
                        }
                    }
                }
                else
                {
                    int min = Math.Min(dt.Count, tempDT.Count); //Which class starts earlier
                    //Compares the courses for the day they are taken on
                    if (dt.Count == min)
                    {
                        flag = compareDays(dt, tempDT);
                    }
                    else
                    {
                        flag = compareDays(tempDT, dt);
                    }
                    if (flag)
                    {
                        return flag;
                    }
                }
            }
            return flag;
        }

        //------------------------------------------------------------------------------
        // helper function for overlap. it's pretty tricky to compare the lists when
        // they are not the same length because of nulls and out of ranges. Can be 
        // eliminated with a more clever algorithm that works of any size lists
        //------------------------------------------------------------------------------
        private bool compareDays(List<DayTime> smaller, List<DayTime> larger)
        {
            for (int k = 0; k < smaller.Count; k++)
            {// go through all days in smaller
                int smallDay = smaller[k].GetDay(); //get day from smaller
                int largeDayIndex = -1;

                for (int j = 0; j < larger.Count; j++)
                { //find that day in larger
                    if (larger[j].GetDay() == smallDay)
                    {
                        largeDayIndex = j;
                        break;
                    }
                }
                //compare that day
                if ((smaller[k].GetStartTime() >= larger[largeDayIndex].GetStartTime() && smaller[k].GetStartTime() <= larger[largeDayIndex].GetEndTime()) ||
                (smaller[k].GetEndTime() >= larger[largeDayIndex].GetStartTime() && smaller[k].GetEndTime() <= larger[largeDayIndex].GetEndTime()))
                {
                    return true;
                }
            }
            return false;
        }

        //------------------------------------------------------------------------------
        // find by retrieving job and looking at when it was scheduled
        // only called if the job actually has prerequisites
        //------------------------------------------------------------------------------
        private int[] GetMostRecentPrereq(List<CourseNode> groups)
        {
            int mostRecentPrereqYear = -1;
            int mostRecentPrereqQuarter = -1;

            for (int i = 1; i < groups.Count; i++)
            {
                for (int j = 0; j < finalPlan.Count; j++)
                {
                    Machine m = finalPlan[j];
                    if (m.GetCurrentJobProcessing() == null || groups[i].prereqs[0] == null)
                    {
                        continue;
                    }

                    if (m.GetCurrentJobProcessing().GetID() == groups[i].prereqs[0].PrerequisiteCourseID)
                    { //found the course
                        if (m.GetCurrentJobProcessing().GetYearScheduled() > mostRecentPrereqYear ||
                           (m.GetCurrentJobProcessing().GetYearScheduled() == mostRecentPrereqYear
                            && m.GetCurrentJobProcessing().GetQuarterScheduled() > mostRecentPrereqQuarter))
                        { //now check if it is more recent
                            mostRecentPrereqYear = m.GetCurrentJobProcessing().GetYearScheduled();
                            mostRecentPrereqQuarter = m.GetCurrentJobProcessing().GetQuarterScheduled();
                        }
                    }
                }
            }
            return new int[] { mostRecentPrereqYear, mostRecentPrereqQuarter };
        }

        //------------------------------------------------------------------------------
        // if for course A you have to take [B, F, K] OR [J, Z], we pick the latter
        // option because we don't want to take a lot of classes; in the long run,
        // this is not always the fastest option so this can be optimized
        //------------------------------------------------------------------------------
        private int GetShortestGroup(List<CourseNode> groups)
        {
            if (groups == null)
            {
                return 0;
            }
            int shortest = int.MaxValue;
            int shortestGroup = int.MaxValue;
            for (int j = 1; j < groups.Count; j++)
            { //find the shortest group that is not null
                var groupCount = 1 + GetShortestGroup(groups[j].prereqs);
                if (groupCount < shortest)
                {
                    shortestGroup = j;
                }
            }//so now we have the shortest list
            return shortestGroup;
        }

        //------------------------------------------------------------------------------
        // if for course A you have to take [B, F, K] OR [J, Z], we pick the latter
        // option because we don't want to take a lot of classes; in the long run,
        // this is not always the fastest option so this can be optimized
        //------------------------------------------------------------------------------
        private int GetAnyGroup(List<CourseNode> groups)
        {
            if (groups == null)
            {
                return 0;
            }
            int shortest = 1;
            var random = new Random();
            var randomGroup = random.Next(groups.Count - 1);
            return Math.Max(shortest, randomGroup);
          
            
        }
        #endregion
    }
}
