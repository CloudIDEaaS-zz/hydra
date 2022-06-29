// file:	IonicFileSystemType.cs
//
// summary:	Implements the ionic file system type class

using AbstraX.FolderStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Attribute for ionic file system type. </summary>
    ///
    /// <remarks>   Ken, 10/11/2020. </remarks>

    public class WebsiteFileSystemTypeAttribute : FileSystemTypeAttribute
    {
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="type"> The type. </param>

        public WebsiteFileSystemTypeAttribute(WebsiteFileSystemType type)
        {
            this.Type = type;
        }
    }

    /// <summary>   Values that represent ionic file system types. </summary>
    ///
    /// <remarks>   Ken, 10/11/2020. </remarks>

    public enum WebsiteFileSystemType
    {
        /// <summary>   An enum constant representing the root option. </summary>
        Root,
        /// <summary>   An enum constant representing the images option. </summary>
        Images,
        /// <summary>   An enum constant representing the styles option. </summary>
        Styles,
        /// <summary>   An enum constant representing the scripts option. </summary>
        Scripts
    }
}
