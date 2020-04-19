﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleEvaluator.ConcreteCriterias
{
    using Models;

    class MaxQuarters : Criteria
    {
        public MaxQuarters(double weight) : base(weight)
        {

        }

        // Validates that the number of quarters scheduled do not exceed 
        // the preferred number of quarters scheduled.
        // Returns the difference between preferred number of quarters and
        // scheduled number of quarters. 
        public override double getResult(ScheduleModel s)
        {
            return Math.Abs(s.Quarters.Count - s.PreferenceSet.MaxQuarters);
        }
    }
}
