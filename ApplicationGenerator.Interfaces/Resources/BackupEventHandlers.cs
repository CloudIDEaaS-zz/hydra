// file:	Resources\SaveBackupEventHandler.cs
//
// summary:	Implements the save backup event handler class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Resources
{
    /// <summary>   Delegate for handling SaveBackup events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>
    ///
    /// <param name="sender">   Source of the event. </param>
    /// <param name="e">        Save backup event information. </param>

    public delegate void SaveBackupEventHandler(object sender, BackupEventArgs e);

    /// <summary>   Delegate for handling RestoreFromBackup events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>
    ///
    /// <param name="sender">   Source of the event. </param>
    /// <param name="e">        Backup event information. </param>

    public delegate void RestoreBackupEventHandler(object sender, BackupEventArgs e);

    /// <summary>   Delegate for handling New events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>
    ///
    /// <param name="sender">   Source of the event. </param>
    /// <param name="e">        Backup event information. </param>

    public delegate void NewEventHandler(object sender, BackupEventArgs e);

    /// <summary>   Additional information for save backup events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>

    public class BackupEventArgs
    {
        /// <summary>   Gets information describing the resource. </summary>
        ///
        /// <value> Information describing the resource. </value>

        public IResourceData ResourceData { get; }

        /// <summary>   Gets the before execute callback. </summary>
        ///
        /// <value> The before execute callback. </value>

        public Action<string, IResourceData> BeforeExecuteCallback { get; }

        /// <summary>   Gets or sets the backup location. </summary>
        ///
        /// <value> The backup location. </value>

        public string BackupLocation { get; set; }

        /// <summary>   Gets or sets the restore location. </summary>
        ///
        /// <value> The restore location. </value>

        public string RestoreLocation { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>
        ///
        /// <param name="resourceData">     Information describing the resource. </param>
        /// <param name="beforeExecuteCallback">  The execute callback. </param>

        public BackupEventArgs(IResourceData resourceData, Action<string, IResourceData> beforeExecuteCallback)
        {
            this.ResourceData = resourceData;
            this.BeforeExecuteCallback = beforeExecuteCallback;
        }
    }
}
