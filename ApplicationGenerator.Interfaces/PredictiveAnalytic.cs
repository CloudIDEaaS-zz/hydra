using AbstraX.DataAnnotations;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   A predictive analytic. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/1/2021. </remarks>

    public class PredictiveAnalytic
    {
        /// <summary>   Gets or sets the pass observed. </summary>
        ///
        /// <value> The pass observed. </value>

        public GeneratorPass PassObserved { get; set; }

        /// <summary>   Gets or sets information describing the member. </summary>
        ///
        /// <value> Information describing the member. </value>

        public MemberInfo MemberInfo { get; set; }

        /// <summary>   Gets or sets the attribute. </summary>
        ///
        /// <value> The user interface attribute. </value>

        public UIAttribute UIAttribute { get; set; }

        /// <summary>   Gets or sets the weight ranking. </summary>
        ///
        /// <value> The weight ranking. </value>

        public int WeightRanking { get; set; }

        /// <summary>   Gets or sets the weight percentage. </summary>
        ///
        /// <value> The weight percentage. </value>

        public float WeightPercentage { get; set; }

        /// <summary>   Gets or sets the definition kind. </summary>
        ///
        /// <value> The definition kind. </value>

        public DefinitionKind DefinitionKind { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is unwinding. </summary>
        ///
        /// <value> True if this  is unwinding, false if not. </value>

        public bool IsUnwinding { get; set; }

        /// <summary>   Gets or sets the reporter. </summary>
        ///
        /// <value> The reporter. </value>

        public IAnalyticsReporter Reporter { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/1/2021. </remarks>
        ///
        /// <param name="memberInfo">       Information describing the member. </param>
        /// <param name="uiAttribute">      The user interface attribute. </param>
        /// <param name="weightRanking">    The weight ranking. </param>

        public PredictiveAnalytic(MemberInfo memberInfo, UIAttribute uiAttribute, int weightRanking)
        {
            this.MemberInfo = memberInfo;
            this.UIAttribute = uiAttribute;
            this.WeightRanking = weightRanking;
        }

        /// <summary>   Gets the status. </summary>
        ///
        /// <value> The status. </value>

        public string Status
        {
            get
            {
                var name = this.MemberInfo.Name.RemoveEndIfMatches("Metadata").RemoveEndIfMatches("Context");

                if (this.UIAttribute.UIHierarchyPath != null)
                {
                    if (this.IsUnwinding)
                    {
                        return string.Format("Finalizing {0} {1}, Path {2}", name, this.UIAttribute.UIKind, this.UIAttribute.UIHierarchyPath);
                    }
                    else
                    {
                        return string.Format("Processing {0} {1}, Path {2}", name, this.UIAttribute.UIKind, this.UIAttribute.UIHierarchyPath);
                    }
                }
                else
                {
                    if (this.IsUnwinding)
                    {
                        return string.Format("Finalizing {0} {1}", name, this.UIAttribute.UIKind);
                    }
                    else
                    {
                        return string.Format("Processing {0} {1}", name, this.UIAttribute.UIKind);
                    }
                }
            }
        }
    }
}
