// file:	ResourceManager.cs
//
// summary:	Implements the resource manager class

using AbstraX.Resources;
using System;

namespace AbstraX
{
    /// <summary>   Interface for resource manager. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 2/28/2021. </remarks>

    public interface IResourceManager : IDisposable
    {
        /// <summary>   Event queue for all listeners interested in onSaveBackup events. </summary>
        event SaveBackupEventHandler OnInitiateSaveBackupEvent;
        /// <summary>   Event queue for all listeners interested in onRestoreBackup events. </summary>
        event RestoreBackupEventHandler OnInitiateRestoreBackupEvent;
        /// <summary>   Event queue for all listeners interested in onInitiateNew events. </summary>
        event NewEventHandler OnInitiateNewEvent;
        /// <summary>   Saves the backup. </summary>
        void InitiateSaveBackup(Action<string, IResourceData> beforeExecuteCallback);
        /// <summary>   Restore backup. </summary>
        void InitiateRestoreBackup(Action<string, IResourceData> beforeExecuteCallback);
        /// <summary>   Initiate new. </summary>
        void InitiateNew(Action<string, IResourceData> beforeExecuteCallback);
        /// <summary>   Gets information describing the resource. </summary>
        ///
        /// <value> Information describing the resource. </value>

        IResourceData ResourceData { get; }

        /// <summary>   Gets the full pathname of the root file. </summary>
        ///
        /// <value> The full pathname of the root file. </value>

        string RootPath { get; }

        /// <summary>   Gets the sass content. </summary>
        ///
        /// <value> The sass content. </value>

        string SassContent { get; }

        /// <summary>   Adds a file resource. </summary>
        ///
        /// <param name="name">     The name. </param>
        /// <param name="filePath"> Full pathname of the file. </param>
        /// <param name="noCopy">   (Optional) True to no copy. </param>

        void AddFileResource(string name, string filePath, bool noCopy = false);

        /// <summary>   Adds a resource to 'value'. </summary>
        ///
        /// <param name="name">     The name. </param>
        /// <param name="value">    The value. </param>

        void AddResource(string name, object value);
        /// <summary>   Clears the resource data. </summary>
        void FreeResourceData();

        /// <summary>   Gets resource value. </summary>
        ///
        /// <param name="name"> The name. </param>
        ///
        /// <returns>   The resource value. </returns>

        string GetResourceValue(string name);

        /// <summary>   Updates this.  </summary>
        ///
        /// <param name="name">     The name. </param>
        /// <param name="value">    The value. </param>

        void Update(string name, object value);
    }
}