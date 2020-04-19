﻿namespace Scheduler.Algorithms
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Contracts;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Implements the Open Shop Scheduling algorithm with genetic algorithm
    /// </summary>
    public class OpenShopGAScheduler : SchedulerBase, IScheduler
    {
        private OpenShopGASchedulerSettings CurrentBestFit = null;
        #region Constructor
        //------------------------------------------------------------------------------
        // 
        // Constructors
        // 
        //------------------------------------------------------------------------------

        public OpenShopGAScheduler(int paramID, bool preferShortest = true, OpenShopGASchedulerSettings currentBestFit = null)
        {
            this.CurrentBestFit = currentBestFit;
            SetUp(paramID);
            MakeStartingPoint();
            InitDegreePlan();
        }
        #endregion

        #region Scheduling Algorithm
        //------------------------------------------------------------------------------
        // Order in which the scheduler processes the jobs is not fixed in advance
        // Of course, we have prerequisites so not everything can be scheduled at random
        // so what we do is define the leaves (stuff we can parallelize) and try to find the best 
        // path for those.
        //------------------------------------------------------------------------------
        public Schedule CreateSchedule(bool preferShortest)
        {
            List<Job> majorCourses = RequiredCourses.GetList(0);
            SortedDictionary<int, List<Job>> jobs = new SortedDictionary<int, List<Job>>();
            for (int i = 0; i < majorCourses.Count; i++)
            {
                Job job = majorCourses[i];
                AddPrerequisites(job, jobs, preferShortest, 0);
            }

            return FindBestSchedule(jobs, 30, 200, 60, this.CurrentBestFit);
        }

        public Schedule FindBestSchedule(SortedDictionary<int, List<Job>> jobs, int level = 20, int populationSize = 100, int topPercentToKeep = 80, OpenShopGASchedulerSettings currentBestFit = null)
        {
            //first, define a population
            var populationSet = new List<Schedule>();
            var rand = new Random();
            for (int i = 0; i < populationSize; i++)
            {
                var chromosome = ScheduleCourses(jobs, true);
                var settings = new OpenShopGASchedulerSettings() { Chromosome = chromosome };
                var sched = new Schedule()
                {
                    Courses = this.Schedule,
                    SchedulerName = nameof(OpenShopGAScheduler),
                    ScheduleSettings = settings,
                    Rating = rand.Next(5)
                };
                populationSet.Add(sched);
            }

            var fittest = SelectFittest(populationSet, level, topPercentToKeep, currentBestFit);
            return fittest.OrderByDescending(s => s.Rating).First();
        }

        public List<Schedule> SelectFittest(List<Schedule> populationSet, int level = 20, int topPercentToKeep = 95, OpenShopGASchedulerSettings currentBestFit = null)
        {
            if (level == 0 || populationSet.Count <= 2)
            {
                return populationSet;
            }
            var rand = new Random();
            Schedule topSchedule = null;
            //select best from population
            if (currentBestFit == null)
            {
                topSchedule = populationSet.OrderByDescending(s => s.Rating).First();
            }
            else
            {
                ScheduleCourse(currentBestFit.Chromosome);
                topSchedule = new Schedule()
                {
                    Courses = this.Schedule,
                    SchedulerName = nameof(OpenShopGAScheduler),
                    ScheduleSettings = currentBestFit,
                    Rating = rand.Next(5)
                };
            }

            var cutoff = populationSet.Count * topPercentToKeep / 100;
            populationSet = populationSet.OrderByDescending(s => s.Rating).Take(cutoff).ToList();
            if (populationSet.Contains(topSchedule))
            {
                populationSet.Remove(topSchedule);
            }
            List<Schedule> offSprings = new List<Schedule>();
            while (populationSet.Count > 0)
            {
                var mate = populationSet.First();
                //cross over
                var crossOvers = GetCrossOvers(topSchedule.ScheduleSettings, mate.ScheduleSettings, 2);
                foreach (var crossOver in crossOvers)
                {
                    //mutate (swap with some other schedule in the population)
                    var randomPop = rand.Next(populationSet.Count - 1);
                    var randomToMutate = populationSet[randomPop];
                    var mutation = GetCrossOvers(crossOver, randomToMutate.ScheduleSettings, 1);

                    ScheduleCourse(mutation.First().Chromosome);
                    var offspring = new Schedule()
                    {
                        Courses = this.Schedule,
                        SchedulerName = nameof(OpenShopGAScheduler),
                        ScheduleSettings = crossOver,
                        Rating = rand.Next(5)
                    };
                    offSprings.Add(offspring);
                }

                populationSet.Remove(mate);

            }

            return SelectFittest(offSprings, level - 1);
        }

        private List<OpenShopGASchedulerSettings> GetCrossOvers(OpenShopGASchedulerSettings parent1, OpenShopGASchedulerSettings parent2, int count)
        {
            var random = new Random();
            var lowest = parent1.Chromosome.Count < parent2.Chromosome.Count ? parent1.Chromosome.Count : parent2.Chromosome.Count;
            var crossOvers = new List<OpenShopGASchedulerSettings>();
            for (int i = 0; i < count; i++)
            {
                var crossOver = new OpenShopGASchedulerSettings()
                {
                    Chromosome = new List<Job>()
                };
                foreach (var job in parent1.Chromosome)
                {
                    crossOver.Chromosome.Add(job);
                }
                var randIndex = random.Next(lowest - 1);

                //swap at this index
                var old = crossOver.Chromosome[randIndex];
                var newVal = crossOver.Chromosome[randIndex];

                crossOver.Chromosome[randIndex] = newVal;
                int jobToReplace = -1;
                for (int j = 0; i < parent1.Chromosome.Count; i++)
                {
                    if (crossOver.Chromosome[i] == newVal)
                    {
                        jobToReplace = i;
                    }
                }

                if (jobToReplace != -1)
                {
                    crossOver.Chromosome[jobToReplace] = old;
                }
                crossOvers.Add(crossOver);
            }


            return crossOvers;
        }

        private List<Job> ScheduleCourses(SortedDictionary<int, List<Job>> jobs, bool mutate)
        {
            var courseDna = new List<Job>();
            //We shouldn't shuffle the actual order of the prereqs, only the courses at the same level
            foreach (var kvp in jobs)
            {
                IEnumerable<Job> orderedJobs = kvp.Value;
                if (mutate)
                {
                    //shuffle the order of courses for each level
                    orderedJobs = kvp.Value.Shuffle();
                }

                foreach (var job in orderedJobs)
                {
                    if (!courseDna.Contains(job))
                    {
                        courseDna.Add(job);
                    }
                    ScheduleCourse(job);
                }
            }

            return courseDna;
        }

        private void ScheduleCourse(List<Job> orderedJobs)
        {
            foreach (var job in orderedJobs)
            {
                ScheduleCourse(job);
            }
        }




        #endregion

    }

    public class OpenShopGASchedulerSettings
    {
        public List<Job> Chromosome { get; set; }
    }
}
