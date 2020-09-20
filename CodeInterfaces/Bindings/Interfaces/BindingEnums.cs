using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces.Bindings
{

    /// <summary>
    /// Supported Operations:
    /// 
    /// GetAll                          (added by default)
    /// GetByID                         (added by default)
    /// GetChildrenForParent            (added by default based on navigation properties)
    /// GetCountOfChildrenForParent     (added by default based on navigation properties)
    ///                         
    /// (remaining operations must be requested by form or control provider):
    /// 
    /// Insert 
    /// Update
    /// Delete
    /// Search
    /// 
    /// </summary>
    [Flags]
    public enum OptionalDataContextOperation
    {
        Insert,
        Update,
        Delete,
        Search,
        AllButSearch = Insert | Update | Delete,
        All = AllButSearch | Search
    }

    [Flags]
    public enum DefaultDataContextOperation
    {
        GetAll,
        GetByID,
        GetChildrenForParent,
        GetCountOfChildrenForParent
    }

    public enum BindingMode
    {
        OneWay,
        TwoWay
    }
}
