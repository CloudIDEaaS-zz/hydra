// file:	MarketingControls\UrlPartPropertyDescriptor.cs
//
// summary:	Implements the URL part property descriptor class

using AbstraX.MarketingControls.SocialMedia;
using AbstraX.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.MarketingControls
{
    /// <summary>   An URL part property descriptor. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>

    public class UrlPart : PropertyDescriptor
    {
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <param name="name">             The name. </param>
        /// <param name="parts">            The parts. </param>
        /// <param name="socialMediaEntry"> The social media entry. </param>
        /// <param name="originalValue">    The original value. </param>
        /// <param name="propertyInfo">     Information describing the property. </param>
        /// <param name="attrs">            The attributes. </param>

        public UrlPart(string name, UrlParts parts, SocialMediaEntry socialMediaEntry, string originalValue, PropertyInfo propertyInfo, Attribute[] attrs) : base(name, attrs)
        {
            this.SocialMediaEntry = socialMediaEntry;
            this.PropertyInfo = propertyInfo;
            this.OriginalValue = originalValue;
            this.Parts = parts;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <param name="name">             The name. </param>
        /// <param name="parts">            The parts. </param>
        /// <param name="tellOthers">       The tell others. </param>
        /// <param name="originalValue">    The original value. </param>
        /// <param name="propertyInfo">     Information describing the property. </param>
        /// <param name="attrs">            The attributes. </param>

        public UrlPart(string name, UrlParts parts, TellOthers tellOthers, string originalValue, PropertyInfo propertyInfo, Attribute[] attrs) : base(name, attrs)
        {
            this.TellOthers = tellOthers;
            this.PropertyInfo = propertyInfo;
            this.OriginalValue = originalValue;
            this.Parts = parts;
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the component this property is bound to.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="T:System.Type" /> that represents the type of component this property is bound
        /// to. When the
        /// <see cref="M:System.ComponentModel.PropertyDescriptor.GetValue(System.Object)" /> or
        /// <see cref="M:System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)" />
        /// methods are invoked, the object specified might be an instance of this type.
        /// </value>

        public override Type ComponentType => throw new NotImplementedException();

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether this property is read-
        /// only.
        /// </summary>
        ///
        /// <value> true if the property is read-only; otherwise, false. </value>

        public override bool IsReadOnly => false;

        /// <summary>   When overridden in a derived class, gets the type of the property. </summary>
        ///
        /// <value> A <see cref="T:System.Type" /> that represents the type of the property. </value>

        public override Type PropertyType => this.SocialMediaEntry.GetUrlPartPropertyType(this.Name);

        /// <summary>   Gets the social media entry. </summary>
        ///
        /// <value> The social media entry. </value>

        public SocialMediaEntry SocialMediaEntry { get; }

        /// <summary>   Gets information describing the property. </summary>
        ///
        /// <value> Information describing the property. </value>

        public PropertyInfo PropertyInfo { get; }

        /// <summary>   Gets the original value. </summary>
        ///
        /// <value> The original value. </value>

        public string OriginalValue { get; }

        /// <summary>   Gets the last value. </summary>
        ///
        /// <value> The last value. </value>

        public string CalculatedValue { get; set; }

        /// <summary>   Gets the parts. </summary>
        ///
        /// <value> The parts. </value>

        public UrlParts Parts { get; }

        /// <summary>   Gets the tell others. </summary>
        ///
        /// <value> The tell others. </value>

        public TellOthers TellOthers { get; }

        /// <summary>
        /// When overridden in a derived class, returns whether resetting an object changes its value.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <param name="component">    The component to test for reset capability. </param>
        ///
        /// <returns>   true if resetting the component changes its value; otherwise, false. </returns>

        public override bool CanResetValue(object component)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, gets the current value of the property on a component.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <param name="component">    The component with the property for which to retrieve the value. </param>
        ///
        /// <returns>   The value of a property for a given component. </returns>

        public override object GetValue(object component)
        {
            var pattern = @"\[(?<part>.+?)\]";
            var regex = new Regex(pattern);
            var propertyName = this.PropertyInfo.Name;
            var replacements = this.Parts.GetReplacements();
            string originalValue;
            string processedValue;

            if (this.SocialMediaEntry != null)
            {
                originalValue = this.OriginalValue;
                processedValue = this.SocialMediaEntry.GetPropertyValue<string>(propertyName);
            }
            else if (this.TellOthers != null)
            {
                originalValue = this.OriginalValue;
                processedValue = this.TellOthers.GetPropertyValue<string>(propertyName);
            }
            else
            {
                DebugUtils.Break();
                return null;
            }

            if (regex.IsMatch(originalValue))
            {
                var matches = regex.Matches(originalValue);

                foreach (var match in matches.Cast<Match>())
                {
                    var group = match.Groups["part"];
                    var partValue = group.Value;

                    if (partValue == this.Name)
                    {
                        var valuePattern = "^" + originalValue.Left(group.Index - 1).DoReplacements(replacements).RegexEscape() + "(?<value>.*?)" + originalValue.RightAt(group.Index + group.Length + 1).DoReplacements(replacements).RegexEscape() + "$";
                        var valueRegex = new Regex(valuePattern);

                        if (valueRegex.IsMatch(processedValue))
                        {
                            var value = valueRegex.GetGroupValue(processedValue, "value");

                            if (value != partValue.SurroundWith("[", "]"))
                            {
                                this.CalculatedValue = value;

                                return value;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// When overridden in a derived class, resets the value for this property of the component to
        /// the default value.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <param name="component">    The component with the property value that is to be reset to the
        ///                             default value. </param>

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a different value.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <param name="component">    The component with the property value that is to be set. </param>
        /// <param name="value">        The new value. </param>

        public override void SetValue(object component, object value)
        {
            var pattern = @"\[(?<part>.+?)\]";
            var regex = new Regex(pattern);
            var propertyName = this.PropertyInfo.Name;
            var propertyOwner = (IPropertyOwner)component;
            var replacements = this.Parts.GetReplacements();
            string originalValue;

            originalValue = this.OriginalValue;

            if (regex.IsMatch(originalValue))
            {
                var matches = regex.Matches(originalValue);

                foreach (var match in matches.Cast<Match>())
                {
                    var group = match.Groups["part"];
                    var partValue = group.Value;

                    if (partValue == this.Name)
                    {
                        var newUrl = originalValue.Left(group.Index - 1).DoReplacements(replacements) + value + originalValue.RightAt(group.Index + group.Length + 1).DoReplacements(replacements);

                        if (this.SocialMediaEntry != null)
                        {
                            this.PropertyInfo.SetValue(this.SocialMediaEntry, newUrl);
                            propertyOwner.PropertyChanged(this, this.SocialMediaEntry);
                        }
                        else if (this.TellOthers != null)
                        {
                            this.PropertyInfo.SetValue(this.TellOthers, newUrl);
                            propertyOwner.PropertyChanged(this, this.TellOthers);
                        }
                        else
                        {
                            DebugUtils.Break();
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, determines a value indicating whether the value of this
        /// property needs to be persisted.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <param name="component">    The component with the property to be examined for persistence. </param>
        ///
        /// <returns>   true if the property should be persisted; otherwise, false. </returns>

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
