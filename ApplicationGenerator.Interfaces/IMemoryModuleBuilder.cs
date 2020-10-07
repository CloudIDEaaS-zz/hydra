// file:	IEntitiesModelGeneratorHandler.cs
//
// summary:	Declares the IEntitiesModelGeneratorHandler interface

using AbstraX.ServerInterfaces;
using CodeInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace AbstraX
{
    /// <summary>   Interface for entities model generator handler. </summary>
    ///
    /// <remarks>   Ken, 10/3/2020. </remarks>

    public interface IMemoryModuleBuilder
    {
        float Priority { get; }
        ModuleBuilder CreateMemoryModuleBuilder(IVSProject project);
    }
}
