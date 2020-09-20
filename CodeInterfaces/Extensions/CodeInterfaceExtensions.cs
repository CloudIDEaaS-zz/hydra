using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Xml.Serialization;
using System.IO;
using CodeInterfaces;
using System.ServiceModel.DomainServices.Server;
using CodePlex.XPathParser;
using CodeInterfaces.XPathBuilder;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using Utils;
using System.Reflection;
using CodeInterfaces.AssemblyInterfaces;
using CodeInterfaces.Bindings;
using CodeInterfaces.Bindings.Interfaces;

namespace CodeInterfaces
{
    [Flags]
    public enum DebugInfoShowOptions
    {
        ShowID = 1,
        ShowCondensedID = 2,
        ShowName = 4,
        ShowDatatype = 8,
        ShowDescription = 16,
        ShowModifiers = 32,
        ShowInCommentMode = 64
    }

    public static class CodeInterfaceExtensions
    {
        public static DebugInfoShowOptions DebugInfoShowOptions { get; set; }
        public static string DebugInfoLineTerminator { get; set; }
        public static string DebugCommentInitiator { get; set; }
        public static int DebugIndent { get; set; }

        static CodeInterfaceExtensions()
        {
            CodeInterfaceExtensions.DebugInfoShowOptions = DebugInfoShowOptions.ShowID | DebugInfoShowOptions.ShowName | DebugInfoShowOptions.ShowDatatype | DebugInfoShowOptions.ShowDescription | CodeInterfaces.DebugInfoShowOptions.ShowModifiers;
            CodeInterfaceExtensions.DebugInfoLineTerminator = "\r\n";
            CodeInterfaceExtensions.DebugCommentInitiator = "/// ";

            CodeInterfaceExtensions.ShowCondensedID = false;
        }

        private static string Prefix
        {
            get
            {
                var prefix = new string('\t', CodeInterfaceExtensions.DebugIndent);

                if (CodeInterfaceExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowInCommentMode))
                {
                    prefix += CodeInterfaceExtensions.DebugCommentInitiator;
                }

                return prefix;
            }
        }

        private static string DoubleSuffix
        {
            get
            {
                var suffix = CodeInterfaceExtensions.DebugInfoLineTerminator;

                if (CodeInterfaceExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowInCommentMode))
                {
                    suffix += new string('\t', CodeInterfaceExtensions.DebugIndent) + CodeInterfaceExtensions.DebugCommentInitiator;
                }

                suffix += CodeInterfaceExtensions.DebugInfoLineTerminator;

                return suffix;
            }
        }

        private static string NewLine
        {
            get
            {
                var suffix = string.Empty;

                if (CodeInterfaceExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowInCommentMode))
                {
                    suffix += new string('\t', CodeInterfaceExtensions.DebugIndent) + CodeInterfaceExtensions.DebugCommentInitiator;
                }

                suffix += CodeInterfaceExtensions.DebugInfoLineTerminator;

                return suffix;
            }
        }

        private static string Suffix
        {
            get
            {
                var suffix = CodeInterfaceExtensions.DebugInfoLineTerminator;

                return suffix;
            }
        }

        public static void ClearIndent()
        {
            CodeInterfaceExtensions.DebugIndent = 0;
        }

        public static bool ShowInCommentsMode
        {
            get
            {
                return CodeInterfaceExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowInCommentMode);
            }

            set
            {
                if (value)
                {
                    CodeInterfaceExtensions.DebugInfoShowOptions = EnumUtils.SetFlag<DebugInfoShowOptions>(CodeInterfaceExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowInCommentMode);
                }
                else
                {
                    CodeInterfaceExtensions.DebugInfoShowOptions = EnumUtils.RemoveFlag<DebugInfoShowOptions>(CodeInterfaceExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowInCommentMode);
                }
            }
        }
        public static bool ShowCondensedID
        {
            get
            {
                return CodeInterfaceExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowCondensedID);
            }

            set
            {
                if (value)
                {
                    CodeInterfaceExtensions.DebugInfoShowOptions = EnumUtils.SetFlag<DebugInfoShowOptions>(CodeInterfaceExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowCondensedID);
                    CodeInterfaceExtensions.DebugInfoShowOptions = EnumUtils.RemoveFlag<DebugInfoShowOptions>(CodeInterfaceExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowID);
                }
                else
                {
                    CodeInterfaceExtensions.DebugInfoShowOptions = EnumUtils.SetFlag<DebugInfoShowOptions>(CodeInterfaceExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowID);
                    CodeInterfaceExtensions.DebugInfoShowOptions = EnumUtils.RemoveFlag<DebugInfoShowOptions>(CodeInterfaceExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowCondensedID);
                }
            }
        }

        public static bool IsLocal(this IBase baseObject)
        {
            if (baseObject is IElement)
            {
                var element = (IElement)baseObject;

                return element.Modifiers.HasFlag(Modifiers.IsLocal);
            }
            else if (baseObject is IAttribute)
            {
                var attribute = (IAttribute)baseObject;

                return attribute.Modifiers.HasFlag(Modifiers.IsLocal);
            }
            else if (baseObject is IOperation)
            {
                var operation = (IOperation)baseObject;

                return operation.Modifiers.HasFlag(Modifiers.IsLocal);
            }
            else
            {
                throw new Exception("IsLocal only applies to elements, attributes, and operations");
            }
        }

        public static string GetQueryMethodForID(this IProviderService service, string id)
        {
            var baseObject = service.GenerateByID(id);
            var method = service.GetType().GetMethods().Single(m =>
            {
                var returnType = m.ReturnType;
                var args = returnType.GetGenericArguments();

                if (returnType.Name == "IQueryable`1" && args.Length == 1 && args.First().Name == baseObject.GetType().Name && m.GetParameters().Length == 0)
                {
                    return true;
                }
                else if (returnType.Name == baseObject.GetType().Name)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            return method.Name;
        }

        public static string MakeID(this IBase baseObject, string predicate)
        {
            var id = "/" + baseObject.GetType().Name + "[@" + predicate + "]";

            if (baseObject.Parent != null)
            {
                id = baseObject.Parent.ID + id;
            }

            return id;
        }

        public static string MakeID(this IBase baseObject, string typeName, string predicate)
        {
            var id = "/" + typeName + "[@" + predicate + "]";

            if (baseObject.Parent != null)
            {
                id = baseObject.Parent.ID + id;
            }

            return id;
        }

        public static BaseType[] GetGenericArguments(this BaseType dataType)
        {
            var type = dataType.UnderlyingType;
            var list = new List<BaseType>();

            foreach (var argType in type.GetGenericArguments())
            {
                if (dataType.SourceParent != null && dataType.SourceParent.FullyQualifiedName == argType.FullName)
                {
                    list.Add(new BaseType(argType, dataType, true));
                }
                else
                {
                    list.Add(new BaseType(argType, dataType));
                }
            }

            return list.ToArray();
        }

        public static BaseType[] GetBaseInterfaces(this BaseType dataType)
        {
            var interfaces = new List<BaseType>();

            foreach (var _interface in dataType.UnderlyingType.GetInterfaces())
            {
                if (dataType.SourceParent != null && dataType.SourceParent.FullyQualifiedName == _interface.FullName)
                {
                    interfaces.Add(new BaseType(_interface, dataType, true));
                }
                else
                {
                    interfaces.Add(new BaseType(_interface, dataType));
                }
            }

            return interfaces.ToArray();
        }

        public static bool Implements(this BaseType type, BaseType implementsType)
        {
            var baseType = type;

            if (!implementsType.IsInterface || type.IsInterface)
            {
                return false;
            }

            while (baseType != null)
            {
                foreach (var interfaceType in type.Interfaces)
                {
                    if (interfaceType.FullyQualifiedName == implementsType.FullyQualifiedName)
                    {
                        return true;
                    }
                    else if (interfaceType.BaseDataType != null && Implements(baseType, interfaceType.BaseDataType))
                    {
                        return true;
                    }
                }

                baseType = baseType.BaseDataType;
            }

            return false;
        }

        public static bool IsSameNamespace(this BaseType type, BaseType otherType)
        {
            var baseType = type.BaseDataType;

            if (type.Namespace == otherType.Namespace)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool InheritsFrom(this BaseType type, BaseType inheritsFromType)
        {
            var baseType = type.BaseDataType;

            if (type.FullyQualifiedName == inheritsFromType.FullyQualifiedName)
            {
                return false;
            }

            while (baseType != null)
            {
                if (baseType.FullyQualifiedName == inheritsFromType.FullyQualifiedName)
                {
                    return true;
                }

                baseType = baseType.BaseDataType;
            }

            return false;
        }

        public static bool InheritsFrom(this Type type, BaseType inheritsFromType)
        {
            var baseType = type.BaseType;

            while (baseType != null)
            {
                if (baseType.FullName == inheritsFromType.FullyQualifiedName)
                {
                    return true;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }

        public static BaseType GetBaseDataType(this Type dataType)
        {
            var type = dataType.BaseType;

            if (type != null)
            {
                return new BaseType(type);
            }

            return null;
        }

        public static string GetProperyName(this IBase baseObject)
        {
            if (baseObject is IGetSetProperty)
            {
                return ((IGetSetProperty)baseObject).PropertyName;
            }
            else
            {
                return baseObject.Name;
            }
        }

        public static string GetDebugInfo(this NavigationItem navigationItem)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("{0}------------------------------------ NavigationItem ------------------------------------{1}", CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.DoubleSuffix);
            builder.AppendFormat("{0}{1}{2}", CodeInterfaceExtensions.Prefix, navigationItem.DebugInfo, CodeInterfaceExtensions.Suffix);
            builder.AppendFormat("{0}CanRead: {1}{2}", CodeInterfaceExtensions.Prefix, navigationItem.CanRead ? "true" : "false", CodeInterfaceExtensions.Suffix);
            builder.AppendFormat("{0}CanWrite: {1}{2}", CodeInterfaceExtensions.Prefix, navigationItem.CanWrite ? "true" : "false", CodeInterfaceExtensions.Suffix);
            builder.AppendFormat("{0}{1}----------------------------------------------------------------------------------{2}", CodeInterfaceExtensions.NewLine, CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.Suffix);

            return builder.ToString();
        }

        public static string GetDebugInfo(this IPropertyBinding binding)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("{0}------------------------------------ PropertyBinding ------------------------------------{1}", CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.DoubleSuffix);
            builder.AppendFormat("{0}Name: {1}{2}", CodeInterfaceExtensions.Prefix, binding.PropertyBindingName, CodeInterfaceExtensions.Suffix);
            builder.AppendFormat("{0}BindingMode: {1}{2}", CodeInterfaceExtensions.Prefix, Enum<BindingMode>.GetName(binding.BindingMode), CodeInterfaceExtensions.Suffix);

            if (binding.BindingSource is IBindingSource)
            {
                var bindingSource = (IBindingSource)binding.BindingSource;

                builder.AppendFormat("{0}BindingSourceType: BindingSource{1}", CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.Suffix);
                builder.AppendFormat("{0}IsSearchable: {1}{2}", CodeInterfaceExtensions.Prefix, bindingSource.IsSearchable ? "true" : "false", CodeInterfaceExtensions.Suffix);

                if (binding.Property != null)
                {
                    builder.AppendFormat("{0}{1}****************** BindingProperty ******************{2}", CodeInterfaceExtensions.NewLine, CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.DoubleSuffix);
                    builder.Append(binding.Property.GetDebugInfo());
                    builder.AppendFormat("{0}{1}*****************************************************{2}", CodeInterfaceExtensions.NewLine, CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.DoubleSuffix);
                }

                builder.AppendFormat("{0}{1}****************** BindingAttribute ******************{2}", CodeInterfaceExtensions.NewLine, CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.DoubleSuffix);
                builder.Append(bindingSource.BindingAttribute.GetDebugInfo());
                builder.AppendFormat("{0}{1}*****************************************************{2}", CodeInterfaceExtensions.NewLine, CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.DoubleSuffix);
            }
            else
            {
                Debugger.Break();
            }

            builder.AppendFormat("{0}{1}----------------------------------------------------------------------------------{2}", CodeInterfaceExtensions.NewLine, CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.Suffix);

            return builder.ToString();
        }

        public static string GetMethodDebugInfo(this IMethodOperation methodOperation)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("{0}{1}------------------------------------ MethodOperation ------------------------------------{1}", CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.DoubleSuffix);
            builder.AppendFormat("{0}Name: {1}{2}", CodeInterfaceExtensions.Prefix, methodOperation.Name, CodeInterfaceExtensions.DoubleSuffix);

            builder.AppendFormat("{0}{1}****************** OperationInfo ******************{2}", CodeInterfaceExtensions.NewLine, CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.DoubleSuffix);
            builder.Append(((IOperation)methodOperation).GetDebugInfo());
            builder.AppendFormat("{0}{1}*****************************************************{2}", CodeInterfaceExtensions.NewLine, CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.DoubleSuffix);

            builder.AppendFormat("{0}{1}{1}----------------------------------------------------------------------------------{1}", CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.Suffix);

            return builder.ToString();
        }

        public static string GetDebugInfo(this IBase baseObject, StringBuilder additional = null)
        {
            var builder = new StringBuilder();

            if (CodeInterfaceExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowID) || CodeInterfaceExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowCondensedID))
            {
                builder.AppendFormat("{0}ID={1}{2}", CodeInterfaceExtensions.Prefix, baseObject.GetID(), CodeInterfaceExtensions.Suffix);
            }

            if (CodeInterfaceExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowName))
            {
                builder.AppendFormat("{0}Name={1}{2}", CodeInterfaceExtensions.Prefix, baseObject.Name, CodeInterfaceExtensions.Suffix);
            }

            if (CodeInterfaceExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowDatatype))
            {
                builder.Append(baseObject.GetDataTypeInfo());
            }

            if (CodeInterfaceExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowDescription))
            {
                builder.Append(baseObject.GetDesignComments());
            }

            if (CodeInterfaceExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowModifiers))
            {
                builder.Append(baseObject.GetModifiersList());
            }

            builder.AppendFormat("{0}**********************************************{1}", CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.Suffix);

            if (additional != null)
            {
                builder.Append(additional.ToString());
            }

            return builder.ToString();
        }

        public static string GetID(this IBase baseObject)
        {
            if (CodeInterfaceExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowCondensedID))
            {
                var parser = new XPathParser<string>();
                var builder = new XPathStringBuilder();
                var id = string.Empty;

                parser.Parse(baseObject.ID, builder);

                id = string.Join("../", builder.AxisElementQueue.Select(e => e.Element));
                id += "[" + builder.AxisElementQueue.Last().Predicates.First().ToString() + "]";

                return id;
            }
            else
            {
                return baseObject.ID;
            }
        }

        public static string GetModifiersList(this IBase baseObject)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("{0}{1}****************** Modifiers ******************{2}", CodeInterfaceExtensions.NewLine, CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.DoubleSuffix);

            foreach (var modifier in Enum<Modifiers>.GetValues())
            {
                if (modifier != Modifiers.Unknown)
                {
                    var name = Enum<Modifiers>.GetName(modifier);

                    builder.AppendFormat("{0}{1}={2}{3}", CodeInterfaceExtensions.Prefix, name, baseObject.Modifiers.HasFlag(modifier) ? "true" : "false", CodeInterfaceExtensions.Suffix);
                }
            }

            builder.Append(CodeInterfaceExtensions.NewLine);

            return builder.ToString();
        }

        public static string GetDesignComments(this IBase baseObject)
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(baseObject.DesignComments))
            {
                builder.AppendFormat("{0}{1}****************** Description ******************{2}", CodeInterfaceExtensions.NewLine, CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.DoubleSuffix);
                builder.AppendFormat("{0}{1}{2}", CodeInterfaceExtensions.Prefix, baseObject.DesignComments, CodeInterfaceExtensions.DoubleSuffix);
            }

            return builder.ToString();
        }

        public static string GetDataTypeInfo(this BaseType baseType)
        {
            var builder = new StringBuilder();

            if (baseType is ScalarType)
            {
                var scalarType = (ScalarType)baseType;

                if (scalarType != null)
                {
                    builder.AppendFormat("{0}{1}****************** DataType ******************{2}", CodeInterfaceExtensions.NewLine, CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.DoubleSuffix);
                    builder.AppendFormat("{0}Type='{1}'{2}", CodeInterfaceExtensions.Prefix, scalarType.Name, CodeInterfaceExtensions.Suffix);
                    builder.AppendFormat("{0}TypeCode='{1}'{2}", CodeInterfaceExtensions.Prefix, Enum.GetName(typeof(TypeCode), scalarType.TypeCode), CodeInterfaceExtensions.DoubleSuffix);
                }
            }
            else
            {
                if (baseType != null)
                {
                    builder.AppendFormat("{0}{1}****************** DataType ******************{2}", CodeInterfaceExtensions.NewLine, CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.DoubleSuffix);
                    builder.AppendFormat("{0}Type='{1}'{2}", CodeInterfaceExtensions.Prefix, baseType.Name, CodeInterfaceExtensions.Suffix);
                    builder.AppendFormat("{0}FQN='{1}'{2}", CodeInterfaceExtensions.Prefix, baseType.FullyQualifiedName, CodeInterfaceExtensions.Suffix);
                    builder.AppendFormat("{0}IsCollectionType='{1}'{2}", CodeInterfaceExtensions.Prefix, baseType.IsCollectionType, CodeInterfaceExtensions.DoubleSuffix);
                }
            }

            return builder.ToString();
        }

        public static string GetDataTypeInfo(this IBase baseObject)
        {
            var builder = new StringBuilder();

            if (baseObject is IAttribute)
            {
                var attribute = (IAttribute)baseObject;

                if (attribute.DataType != null)
                {
                    builder.AppendFormat("{0}{1}****************** DataType ******************{2}", CodeInterfaceExtensions.NewLine, CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.DoubleSuffix);
                    builder.AppendFormat("{0}Type='{1}'{2}", CodeInterfaceExtensions.Prefix, attribute.DataType.Name, CodeInterfaceExtensions.Suffix);
                    builder.AppendFormat("{0}TypeCode='{1}'{2}", CodeInterfaceExtensions.Prefix, Enum.GetName(typeof(TypeCode), attribute.DataType.TypeCode), CodeInterfaceExtensions.DoubleSuffix);
                }
            }
            else if (baseObject is IElement)
            {
                var element = (IElement)baseObject;

                if (element.DataType != null)
                {
                    builder.AppendFormat("{0}{1}****************** DataType ******************{2}", CodeInterfaceExtensions.NewLine, CodeInterfaceExtensions.Prefix, CodeInterfaceExtensions.DoubleSuffix);
                    builder.AppendFormat("{0}Type='{1}'{2}", CodeInterfaceExtensions.Prefix, element.DataType.Name, CodeInterfaceExtensions.Suffix);
                    builder.AppendFormat("{0}FQN='{1}'{2}", CodeInterfaceExtensions.Prefix, element.DataType.FullyQualifiedName, CodeInterfaceExtensions.Suffix);
                    builder.AppendFormat("{0}IsCollectionType='{1}'{2}", CodeInterfaceExtensions.Prefix, element.DataType.IsCollectionType, CodeInterfaceExtensions.DoubleSuffix);
                }
            }

            return builder.ToString();
        }

        public static string GetRootID(string id)
        {
            var regex = new Regex(@"/(?<providertype>\w+?)\[\@URL=\'(?<url>.*?)\'\]");

            if (regex.IsMatch(id))
            {
                var match = regex.Match(id);
                var rootID = "/" + match.Groups["providertype"].Value + "[@URL='" + match.Groups["url"].Value + "']";

                return rootID;
            }
            else
            {
                return null;
            }
        }

        public static IBase GenerateByID(this IProviderService service, string id)
        {
            var queue = new Queue<string>();
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();

            parser.Parse(id, builder);

            if (builder.AxisElementQueue.Count == 1)
            {
                var axisElement = builder.AxisElementQueue.Single();

                var method = service.GetType().GetMethods().Single(m => m.ReturnType.Name == axisElement.Element && m.GetParameters().Length == 0);

                service.LogGenerateByID(id, method);

                var rootObject = (IBase)method.Invoke(service, null);

                service.PostLogGenerateByID();

                return rootObject;
            }
            else
            {
                var axisElement = builder.AxisElementQueue.Last();

                var method = service.GetType().GetMethods().Single(m => m.ReturnType.Name == "IQueryable`1" && m.GetParameters().Length == 0 && m.ReturnType.GetGenericArguments().Any(a => a.Name == axisElement.Element));

                service.LogGenerateByID(id, method);

                var results = (IQueryable<IBase>)method.Invoke(service, null);

                service.PostLogGenerateByID();

                return results.Where(b => b.ID == id).Single();
            }
        }

        public static IQueryable<T> GenerateByID<T>(this IProviderService service, string id)
        {
            var queue = new Queue<string>();
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();

            parser.Parse(id, builder);

            var axisElement = builder.AxisElementQueue.Last();

            var method = service.GetType().GetMethods().Single(m => m.ReturnType.Name == "IQueryable`1" && m.GetParameters().Length == 0 && m.ReturnType.GetGenericArguments().Any(a => a.Name == axisElement.Element));

            service.LogGenerateByID(id, method);

            var results = (IQueryable<IBase>)method.Invoke(service, null);

            service.PostLogGenerateByID();

            return results.Where(b => b.ID == id).Cast<T>();
        }
    }
}
