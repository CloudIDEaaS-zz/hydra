using AbstraX.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.TemplateObjects
{
    /// <summary>   An application UI hierarchy node object. </summary>
    ///
    /// <remarks>   Ken, 10/3/2020. </remarks>

    [DebuggerDisplay(" { DebugInfo }")]
    public class AppUIHierarchyNodeObject : UIHierarchyNodeObject
    {
        /// <summary>   Gets or sets the entities. </summary>
        ///
        /// <value> The entities. </value>


        /// <summary>   Gets or sets the inherent entities. </summary>
        ///
        /// <value> The inherent entities. </value>

        public List<EntityObject> InherentEntities { get; set; }

        /// <summary>   Gets or sets all entities. </summary>
        ///
        /// <value> all entities. </value>

        public List<EntityObject> AllEntities { get; set; }

        /// <summary>   Gets or sets the entities module builder. </summary>
        ///
        /// <value> The entities module builder. </value>

        public ModuleBuilder EntitiesModuleBuilder { get; set; }

        /// <summary>   Gets or sets the top user interface hierarchy node objects. </summary>
        ///
        /// <value> The top user interface hierarchy node objects. </value>

        public List<UIHierarchyNodeObject> TopUIHierarchyNodeObjects { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/7/2020. </remarks>

        public AppUIHierarchyNodeObject()
        {
            this.AllEntities = new List<EntityObject>();
            this.InherentEntities = new List<EntityObject>();
            this.TopUIHierarchyNodeObjects = new List<UIHierarchyNodeObject>();
        }
    }
}
