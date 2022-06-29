using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Threading.Tasks;
using Utils;
using CoreShim.Reflection.JsonTypes;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using CustomAttributeData = System.Reflection.CustomAttributeData;
using System.Security.Policy;
using System.Security;
using NetCoreReflectionShim.Agent;
using TypeExtensions = Utils.TypeExtensions;

namespace CoreShim.Reflection
{
    public class PropertyInfoShim : PropertyInfo
    {
        private PropertyInfoJson propertyInfo;
        private INetCoreReflectionAgent agent;
        private string parentIdentifier;
        private Attribute[] customAttributes;
        private PropertyInfo[] properties;

        public PropertyInfoShim(PropertyInfoJson propertyInfo, string parentIdentifier, INetCoreReflectionAgent agent)
        {
            this.propertyInfo = propertyInfo;
            this.agent = agent;
            this.parentIdentifier = parentIdentifier;
        } 

        public override System.Reflection.MemberTypes MemberType
        { 
            get
            {
                return EnumUtils.GetValue<System.Reflection.MemberTypes>(propertyInfo.MemberTypeEnum);
            }
        }

        public override Type PropertyType 
        { 
            get
            {
                Type propertyType = null;

                if (propertyInfo.PropertyType.Contains("`"))
                {
                    var genericBaseType = propertyInfo.PropertyType.Left(propertyInfo.PropertyType.LastIndexOf("`"));
                    var genericTypeArgText = propertyInfo.PropertyType.RegexGet(@"^.*?\[(?<genericTypeArgument>.*?)\]$", "genericTypeArgument");
                    var genericTypeArgName = agent.MapTypeName(genericTypeArgText);
                    var typeName = agent.MapTypeName(genericBaseType + "`1");
                    Type genericTypeArg;

                    propertyType = agent.GetType(typeName);
                    genericTypeArg = agent.GetType(genericTypeArgName);
                    propertyType = propertyType.MakeGenericType(genericTypeArg);
                }
                else
                {
                    propertyType = TypeExtensions.GetType(propertyInfo.PropertyType);

                    if (propertyType == null)
                    {
                        if (agent.CachedTypes.ContainsKey(propertyInfo.PropertyType))
                        {
                            propertyType = agent.CachedTypes[propertyInfo.PropertyType];
                        }
                        else
                        {
                            propertyType = typeof(object);
                        }
                    }
                }

                return propertyType;
            }
        }

        public override System.Reflection.PropertyAttributes Attributes
        { 
            get
            {
                return EnumUtils.GetValue<System.Reflection.PropertyAttributes>(propertyInfo.AttributesEnum);
            }
        }

        public override bool CanRead 
        { 
            get
            {
                return propertyInfo.CanRead;
            }
        }

        public override bool CanWrite 
        { 
            get
            {
                return propertyInfo.CanWrite;
            }
        }

        public override MethodInfo GetMethod 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public override MethodInfo SetMethod 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public new bool IsSpecialName 
        { 
            get
            {
                return propertyInfo.IsSpecialName;
            }
        }

        public override string Name 
        { 
            get
            {
                return propertyInfo.Name;
            }
        }

        public override Type DeclaringType 
        { 
            get
            {
                Type declaringType = null;

                if (propertyInfo.DeclaringType.Contains("`"))
                {
                    var genericBaseType = propertyInfo.DeclaringType.Left(propertyInfo.DeclaringType.LastIndexOf("`"));
                    var typeName = agent.MapTypeName(genericBaseType + "`1");

                    declaringType = agent.GetType(typeName);
                }
                else
                {
                    declaringType = TypeExtensions.GetType(propertyInfo.DeclaringType);

                    if (declaringType == null)
                    {
                        if (agent.CachedTypes.ContainsKey(propertyInfo.DeclaringType))
                        {
                            declaringType = agent.CachedTypes[propertyInfo.DeclaringType];
                        }
                        else
                        {
                            declaringType = typeof(object);
                        }
                    }
                }

                return declaringType;
            }
        }

        public override Type ReflectedType 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int MetadataToken 
        { 
            get
            {
                return propertyInfo.MetadataToken;
            }
        }

        public override Module Module 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }
        public override int GetHashCode()
        {
            return propertyInfo.GetHashCodeMember;
        }
        public override object GetConstantValue()
        {
            throw new NotImplementedException();
        }
        public override object GetRawConstantValue()
        {
            throw new NotImplementedException();
        }
        public new object GetValue(object obj)
        {
            throw new NotImplementedException();
        }
        public override object GetValue(object obj, Object[] index)
        {
            throw new NotImplementedException();
        }
        public new void SetValue(object obj, object value)
        {
            throw new NotImplementedException();
        }
        public override void SetValue(object obj, object value, Object[] index)
        {
            throw new NotImplementedException();
        }
        public override Type[] GetRequiredCustomModifiers()
        {
            throw new NotImplementedException();
        }
        public override Type[] GetOptionalCustomModifiers()
        {
            throw new NotImplementedException();
        }
        public new MethodInfo[] GetAccessors()
        {
            throw new NotImplementedException();
        }
        public new MethodInfo GetGetMethod()
        {
            throw new NotImplementedException();
        }
        public new MethodInfo GetSetMethod()
        {
            throw new NotImplementedException();
        }
        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, Object[] index, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            throw new NotImplementedException();
        }
        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }
        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }
        public override ParameterInfo[] GetIndexParameters()
        {
            throw new NotImplementedException();
        }
        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, Object[] index, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            throw new NotImplementedException();
        }

        public override Object[] GetCustomAttributes(bool inherit)
        {
            if (customAttributes == null)
            {
                customAttributes = (Attribute[])agent.MemberInfo_GetCustomAttributes(string.Format("{0}[@MetadataToken={1}]", parentIdentifier, propertyInfo.MetadataToken.ToString()), inherit);
            }

            return customAttributes;
        }
        public override Object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            if (customAttributes == null)
            {
                customAttributes = (Attribute[])agent.MemberInfo_GetCustomAttributes(string.Format("{0}[@MetadataToken={1}]", parentIdentifier, propertyInfo.MetadataToken.ToString()), inherit);
            }

            return customAttributes;
        }
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return propertyInfo.ToStringMember;
        }
    }
}
