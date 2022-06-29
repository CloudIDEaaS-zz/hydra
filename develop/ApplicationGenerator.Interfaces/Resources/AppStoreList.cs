// file:	Resources\AppStoreList.cs
//
// summary:	Implements the application store list class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Resources
{
    /// <summary>   List of application stores. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/7/2021. </remarks>

    public class AppStoreList
    {
        /// <summary>   Gets or sets the entries. </summary>
        ///
        /// <value> The entries. </value>

        public AppStoreEntry[] AppStores { get; set; }

        /// <summary>   Sets the pathname of the working directory. </summary>
        ///
        /// <value> The pathname of the working directory. </value>

        public string WorkingDirectory
        {
            set
            {
                foreach (var appStore in this.AppStores)
                {
                    appStore.WorkingDirectory = value;
                }
            }
        }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/7/2021. </remarks>

        public AppStoreList()
        {
            this.AppStores = new AppStoreEntry[0];
        }
    }
}
