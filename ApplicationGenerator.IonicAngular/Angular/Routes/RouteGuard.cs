using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Angular.Routes
{
    [DebuggerDisplay(" { Name } ")]
    public class RouteGuard : Component, IRouteGuard
    {
        public UINavigationNameAttribute UINavigationNameAttribute { get; }
        public string RouteName { get; }
        public string DefaultRouteName { get; }
        public RouteGuardKind Kind { get; }
        public bool CheckStorage { get; }
        public string StorageVariable { get; }
        public bool IsHome { get; }
        public string ImportPath { get; set; }
        public Dictionary<string, List<IRouteGuard>> AllRouteGuards { get; }

        public RouteGuard(IBase baseObject, DataAnnotations.UINavigationNameAttribute uiNavigationName, string routeName, string defaultRouteName, RouteGuardKind routeGuardKind, bool checkStorage, string storageVariable, bool isHome, Dictionary<string, List<RouteGuard>> routeGuards) : base(uiNavigationName.Name, UIKind.None)
        {
            this.BaseObject = baseObject;
            this.UINavigationNameAttribute = uiNavigationName;
            this.RouteName = routeName;
            this.Name = "Check" + routeName;
            this.DefaultRouteName = defaultRouteName;
            this.Kind = routeGuardKind;
            this.CheckStorage = checkStorage;
            this.StorageVariable = storageVariable;
            this.IsHome = isHome;
            this.AllRouteGuards = routeGuards.ToDictionary(r => r.Key, r => r.Value.Cast<IRouteGuard>().ToList());
        }
    }
}
