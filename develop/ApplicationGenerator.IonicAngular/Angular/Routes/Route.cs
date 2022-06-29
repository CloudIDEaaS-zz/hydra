using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Angular.Routes
{
    [DebuggerDisplay(" { Code } ")]
    public class Route
    {
        public string Path { get; set; }
        public string RedirectTo { get; set; }
        public string PathMatch { get; set; }
        public LoadChildren LoadChildren { get; set; }
        public Page Component { get; set; }
        public List<Component> CanLoad { get; }
        public List<Component> CanActivate { get; }
        public List<Route> Children { get; }

        public Route()
        {
            this.CanLoad = new List<Component>();
            this.CanActivate = new List<Component>();
            this.Children = new List<Route>();
        }

        public string Code
        {
            get
            {
                var builder = new StringBuilder();

                builder.AppendLineSpaceIndent(2, "{");

                builder.AppendLineFormatSpaceIndent(4, "path: \"{0}\",", this.Path);

                if (this.Component != null)
                {
                    builder.AppendLineFormatSpaceIndent(4, "component: {0},", this.Component.Name);
                }

                if (!this.RedirectTo.IsNullOrEmpty())
                {
                    builder.AppendLineFormatSpaceIndent(4, "redirectTo: \"/{0}\",", this.RedirectTo);
                }

                if (!this.PathMatch.IsNullOrEmpty())
                {
                    builder.AppendLineFormatSpaceIndent(4, "pathMatch: \"{0}\",", this.PathMatch);
                }

                if (this.LoadChildren != null)
                {
                    if (this.CanLoad.Count > 0)
                    {
                        builder.AppendLineFormatSpaceIndent(4, "loadChildren: {0},", this.LoadChildren.FunctionCode);
                        builder.AppendLineFormatSpaceIndent(4, "canLoad: [{0}],", this.CanLoad.Select(c => c.Name).ToCommaDelimitedList());
                    }
                    else
                    {
                        builder.AppendLineFormatSpaceIndent(4, "loadChildren: {0}", this.LoadChildren.FunctionCode);
                    }
                }

                if (this.Children.Count > 0)
                {
                    builder.AppendLineFormatSpaceIndent(4, "children: [");

                    foreach (var childRoute in this.Children)
                    {
                        var childCode = childRoute.Code.IndentLinesSpaces(4);

                        builder.AppendLineFormat("{0},", childCode);
                    }

                    builder.RemoveEnd(1);

                    builder.AppendLineFormatSpaceIndent(4, "]");
                }

                builder.AppendSpaceIndent(2, "}");

                return builder.ToString();
            }
        }
    }
}
