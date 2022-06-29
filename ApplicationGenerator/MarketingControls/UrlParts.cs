// file:	MarketingControls\UrlParts.cs
//
// summary:	Implements the URL parts class

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.MarketingControls
{
    /// <summary>   An URL parts. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>

    public class UrlParts : Dictionary<Group, UrlPart>, ICustomTypeDescriptor
    {
        private IPropertyOwner propertyOwner;

        /// <summary>   Gets or sets a value indicating whether the replacements built. </summary>
        ///
        /// <value> True if replacements built, false if not. </value>

        public bool ReplacementsBuilt { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2021. </remarks>
        ///
        /// <param name="propertyOwner">    The owner of the property. </param>

        public UrlParts(IPropertyOwner propertyOwner)
        {
            this.propertyOwner = propertyOwner;
        }

        /// <summary>   Returns the properties for this instance of a component. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> that represents the
        /// properties for this component instance.
        /// </returns>

        public PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(this.Values.ToArray());
        }

        /// <summary>
        /// Returns a collection of custom attributes for this instance of a component.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <returns>
        /// An <see cref="T:System.ComponentModel.AttributeCollection" /> containing the attributes for
        /// this object.
        /// </returns>

        public AttributeCollection GetAttributes()
        {
            return new AttributeCollection();
        }

        /// <summary>   Builds the replacements. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/13/2021. </remarks>

        public void BuildReplacements()
        {
            var replacements = new Dictionary<string, string>();

            foreach (var part in this.Values)
            {
                replacements.Add(part.Name.SurroundWith(@"\[", "]"), ".*?");
                replacements.Add(part.Name.SurroundWith("[", "]"), ".*?");
            }

            foreach (var part in this.Values)
            {
                var pattern = @"\[(?<part>.+?)\]";
                var regex = new Regex(pattern);
                var propertyName = part.PropertyInfo.Name;
                string originalValue;
                string processedValue;

                if (part.SocialMediaEntry != null)
                {
                    originalValue = part.OriginalValue;
                    processedValue = part.SocialMediaEntry.GetPropertyValue<string>(propertyName);
                }
                else if (part.TellOthers != null)
                {
                    originalValue = part.OriginalValue;
                    processedValue = part.TellOthers.GetPropertyValue<string>(propertyName);
                }
                else
                {
                    DebugUtils.Break();
                    return;
                }

                if (regex.IsMatch(originalValue))
                {
                    var matches = regex.Matches(originalValue);

                    foreach (var match in matches.Cast<Match>())
                    {
                        var group = match.Groups["part"];
                        var partValue = group.Value;

                        if (partValue == part.Name)
                        {
                            var valuePattern = "^" + originalValue.Left(group.Index - 1).RegexEscape().DoReplacements(replacements) + "(?<value>.*?)" + originalValue.RightAt(group.Index + group.Length + 1).RegexEscape().DoReplacements(replacements) + "$";
                            var valueRegex = new Regex(valuePattern);

                            if (valueRegex.IsMatch(processedValue))
                            {
                                var value = valueRegex.GetGroupValue(processedValue, "value");

                                if (value != partValue.SurroundWith("[", "]"))
                                {
                                    part.CalculatedValue = value;
                                }

                                break;
                            }
                        }
                    }
                }
            }

            this.ReplacementsBuilt = true; 
        }

        /// <summary>   Returns the class name of this instance of a component. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <returns>   The class name of the object, or null if the class does not have a name. </returns>

        public string GetClassName()
        {
            return nameof(UrlParts);
        }

        /// <summary>   Returns the name of this instance of a component. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <returns>   The name of the object, or null if the object does not have a name. </returns>

        public string GetComponentName()
        {
            return nameof(UrlParts);
        }

        /// <summary>   Returns a type converter for this instance of a component. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <returns>
        /// A <see cref="T:System.ComponentModel.TypeConverter" /> that is the converter for this object,
        /// or null if there is no <see cref="T:System.ComponentModel.TypeConverter" /> for this object.
        /// </returns>

        public TypeConverter GetConverter()
        {
            return null;
        }

        /// <summary>   Returns the default event for this instance of a component. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <returns>
        /// An <see cref="T:System.ComponentModel.EventDescriptor" /> that represents the default event
        /// for this object, or null if this object does not have events.
        /// </returns>

        public EventDescriptor GetDefaultEvent()
        {
            throw new NotImplementedException();
        }

        /// <summary>   Returns the default property for this instance of a component. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptor" /> that represents the default
        /// property for this object, or null if this object does not have properties.
        /// </returns>

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        /// <summary>   Returns an editor of the specified type for this instance of a component. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <param name="editorBaseType">   A <see cref="T:System.Type" /> that represents the editor for
        ///                                 this object. </param>
        ///
        /// <returns>
        /// An <see cref="T:System.Object" /> of the specified type that is the editor for this object,
        /// or null if the editor cannot be found.
        /// </returns>

        public object GetEditor(Type editorBaseType)
        {
            return null;
        }

        /// <summary>   Returns the events for this instance of a component. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <returns>
        /// An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> that represents the
        /// events for this component instance.
        /// </returns>

        public EventDescriptorCollection GetEvents()
        {
            return new EventDescriptorCollection(new EventDescriptor[0]);
        }

        /// <summary>
        /// Returns the events for this instance of a component using the specified attribute array as a
        /// filter.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <param name="attributes">   An array of type <see cref="T:System.Attribute" /> that is used
        ///                             as a filter. </param>
        ///
        /// <returns>
        /// An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> that represents the
        /// filtered events for this component instance.
        /// </returns>

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return new EventDescriptorCollection(new EventDescriptor[0]);
        }

        /// <summary>
        /// Returns the properties for this instance of a component using the attribute array as a filter.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <param name="attributes">   An array of type <see cref="T:System.Attribute" /> that is used
        ///                             as a filter. </param>
        ///
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> that represents the
        /// filtered properties for this component instance.
        /// </returns>

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(this.Values.ToArray());
        }

        /// <summary>
        /// Returns an object that contains the property described by the specified property descriptor.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <param name="pd">   A <see cref="T:System.ComponentModel.PropertyDescriptor" /> that
        ///                     represents the property whose owner is to be found. </param>
        ///
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the owner of the specified property.
        /// </returns>

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return propertyOwner;
        }
    }
}
