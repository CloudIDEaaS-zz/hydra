// file:	EntityDomainModel.cs
//
// summary:	Implements the entity domain model class

using AbstraX.TemplateObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   A data Model for the entity domain. </summary>
    ///
    /// <remarks>   Ken, 10/1/2020. </remarks>

    public class EntityDomainModel : TsvParserHost<EntityDomainRecord>
    {
        /// <summary>   Gets or sets the entities. </summary>
        ///
        /// <value> The entities. </value>

        public List<EntityObject> Entities { get; set; }

        /// <summary>   Parse file. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="path">             Full pathname of the file. </param>
        /// <param name="textReplacements"> The text replacements. </param>
        ///
        /// <returns>   A List&lt;TRecord&gt; </returns>

        public override List<EntityDomainRecord> ParseFile(string path, Dictionary<string, string> textReplacements)
        {
            var records = base.ParseFile(path, textReplacements);
            EntityObject lastEntity = null;

            this.Entities = new List<EntityObject>();

            foreach (var record in records)
            {
                if (record.EntityName != null)
                {
                    var entity = new EntityObject(record.EntityName, record.ParentDataItem);

                    if (record.Properties != null)
                    {
                        entity.Properties.AddRange(record.Properties);
                    }

                    this.Entities.Add(entity);

                    lastEntity = entity;
                }
                else if (record.AttributeName != null)
                {
                    var attribute = new AttributeObject(record.AttributeName, record.AttributeType);

                    if (record.Properties != null)
                    {
                        attribute.Properties.AddRange(record.Properties);
                    }

                    lastEntity.Attributes.Add(attribute);
                }
                else
                {
                    DebugUtils.Break();
                }
            }

            return records;
        }
    }
}
