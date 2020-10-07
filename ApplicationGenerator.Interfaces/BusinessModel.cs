// file:	BusinessModel.cs
//
// summary:	Implements the business model class

using AbstraX.TemplateObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   A data Model for the business. </summary>
    ///
    /// <remarks>   Ken, 10/1/2020. </remarks>

    public class BusinessModel : TsvParserHost<BusinessModelRecord>
    {
        /// <summary>   Gets or sets the records. </summary>
        ///
        /// <value> The records. </value>

        public List<BusinessModelRecord> Records { get; private set; }

        /// <summary>   Gets or sets the top level object. </summary>
        ///
        /// <value> The top lfevel object. </value>

        public BusinessModelObject TopLevelObject { get; set; }

        /// <summary>   Parse file. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="path">             Full pathname of the file. </param>
        /// <param name="textReplacements"> The text replacements. </param>
        ///
        /// <returns>   A List&lt;TRecord&gt; </returns>

        public override List<BusinessModelRecord> ParseFile(string path, Dictionary<string, string> textReplacements)
        {
            var allObjects = new List<BusinessModelObject>();

            this.Records = base.ParseFile(path, textReplacements);

            foreach (var record in this.Records)
            {
                BusinessModelObject obj;

                if (allObjects.Any(o => o.Id == record.Id))
                {
                    obj = allObjects.Single(o => o.Id == record.Id);
                    allObjects.Add(obj);
                }
                else
                {
                    obj = record.CreateCopy<BusinessModelObject>();
                }

                if (record.ParentId == 0)
                {
                    this.TopLevelObject = obj;
                }

                foreach (var childRecord in this.Records.Where(r => r.ParentId == obj.Id))
                {
                    BusinessModelObject childObject;

                    if (allObjects.Any(o => o.Id == childRecord.Id))
                    {
                        childObject = allObjects.Single(o => o.Id == childRecord.Id);
                    }
                    else
                    {
                        childObject = childRecord.CreateCopy<BusinessModelObject>();
                        allObjects.Add(childObject);
                    }

                    if (childObject.ShadowItem != 0)
                    {
                        var shadowRecord = this.Records.Single(r => r.Id == childObject.ShadowItem);

                        childObject.Name = shadowRecord.Name;
                        childObject.Level = shadowRecord.Level;
                        childObject.ClassName = shadowRecord.ClassName;
                    }

                    obj.Children.Add(childObject);
                }
            }

            return Records;
        }
    }
}
