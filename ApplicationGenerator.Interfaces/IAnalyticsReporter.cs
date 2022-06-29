// file:	AbtraXExtensions.cs
//
// summary:	Implements the abtra x coordinate extensions class

using System.Collections.Generic;
using System.Reflection;

namespace AbstraX
{
    /// <summary>   Interface for analytics reporter. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/1/2021. </remarks>

    public interface IAnalyticsReporter
    {
        /// <summary>   Reports an observation. </summary>
        ///
        /// <param name="analytic"> The analytic. </param>

        void ReportObservation(PredictiveAnalytic analytic);
    }
}