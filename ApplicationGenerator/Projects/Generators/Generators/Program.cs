using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityProvider.Web.Entities;
using Utils;

namespace Generators
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileInfo = new FileInfo(@"C:\Projects\RipleyEntities\Ripley.edmx");
            var model = new Model(fileInfo);

            foreach (var container in model.Containers)
            {
                foreach (var entitySet in container.EntitySets)
                {
                    foreach (var entity in entitySet.Entities)
                    {
                        PrintEntity(entity, 0);
                    }
                }
            }
        }

        private static void PrintEntity(EntityType entity, int tabs)
        {
            var tabText = new string('\t', tabs);

            Debug.WriteLine("{0}{1}", tabText, entity.Name);

            foreach (var property in entity.Properties)
            {
                tabText = new string('\t', tabs + 1);

                Debug.WriteLine("{0}{1} ({2})", tabText, property.Name, property.DataType.Name);
            }

            foreach (var property in entity.NavigationProperties)
            {
                tabText = new string('\t', tabs + 1);

                Debug.WriteLine("{0}{1} ({2})", tabText, property.Name);

                foreach (var childEntity in property.ChildEntities)
                {
                    PrintEntity(childEntity, tabs + 2);
                }
            }
        }
    }
}
