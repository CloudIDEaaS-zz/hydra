#region License
/* 
 * Copyright (C) 1999-2015 John K�ll�n.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Decompiler.Core;
using System;
using System.Collections.Generic;

namespace Decompiler.Core
{
	/// <summary>
	/// Abstract base class for image loaders. These examine a raw image, and generate a new,
    /// relocated image.
	/// </summary>
	public abstract class ImageLoader
	{
        public ImageLoader(IServiceProvider services, string filename, byte[] imgRaw)
        {
            this.Services = services;
            this.Filename = filename;
            this.RawImage = imgRaw;
        }

        public IServiceProvider Services { get; private set; }

        /// <summary>
        /// If nothing else is specified, this is the address at which the image will be loaded.
        /// </summary>
        public abstract Address PreferredBaseAddress { get; set; }

        /// <summary>
        /// Optional loader-specific argument specified in app.config.
        /// </summary>
        public string Argument { get; set; }

        /// <summary>
        /// The image as it appears on the storage medium before being loaded.
        /// </summary>
        public byte[] RawImage { get; private set; }

        /// <summary>
        /// The name of the file the image was loaded from.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Loads the header of the executable, so that its contents can be summarized. 
        /// </summary>
        /// <returns></returns>
        public ImageHeader LoadHeader(string argument) { throw new NotImplementedException();  }

        /// <summary>
		/// Loads the image into memory starting at the specified address
		/// </summary>
		/// <param name="addrLoad">Base address of program image</param>
		/// <returns></returns>
        public abstract Program Load(Address addrLoad);

        /// <summary>
        /// Performs fix-ups of the loaded image, adding findings to the supplied collections.
        /// </summary>
        /// <param name="addrLoad">The address at which the program image is loaded.</param>
        /// <returns></returns>
		public abstract RelocationResults Relocate(Address addrLoad);
    }
}
