using NetCoreReflectionShim.CodeGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace NetCoreReflectionShim.CodeGen
{
    public class ApiMember
    {
        public Type Type { get; }
        public MemberInfo MemberInfo { get; }
        public bool CacheResult { get; }
        public bool NoShim { get; }
        public bool MangleName { get; }
        public bool NoCacheResult { get; set; }
        public Type ReturnElementTypeOverride { get; set; }
        public string ReturnElementTypeTextOverride { get; set; }

        public ApiMember(MemberInfo memberInfo, bool cacheResult = false, bool noShim = false, bool mangleName = false)
        {
            this.MemberInfo = memberInfo;
            this.CacheResult = cacheResult;
            this.NoShim = noShim;
            this.MangleName = mangleName;
        }

        public ApiMember(Type type, MemberInfo memberInfo, bool cacheResult = false, bool noShim = false, bool mangleName = false)
        {
            this.Type = type;
            this.MemberInfo = memberInfo;
            this.CacheResult = cacheResult;
            this.NoShim = noShim;
            this.MangleName = mangleName;
        }

        public Type ClientReturnType
        {
            get
            {
                Type returnType;

                switch (this.MemberInfo)
                {
                    case MethodInfo methodInfo:

                        returnType = methodInfo.ReturnType;
                        break;

                    case PropertyInfo propertyInfo:

                        returnType = propertyInfo.PropertyType;
                        break;

                    default:

                        DebugUtils.Break();
                        returnType = null;
                        break;
                }

                return returnType;
            }
        }

        public string ClientReturnTypeName
        {
            get
            {
                string returnType;

                switch (this.MemberInfo)
                {
                    case MethodInfo methodInfo:

                        returnType = methodInfo.ReturnType.GenerateTypeString();
                        break;

                    case PropertyInfo propertyInfo:

                        returnType = propertyInfo.PropertyType.GenerateTypeString();
                        break;

                    default:

                        DebugUtils.Break();
                        returnType = null;
                        break;
                }

                return returnType;
            }
        }

        public string CommandMember
        {
            get
            {
                string commandMember;
                var declaringType = this.MemberInfo.GetOwningType(this.Type);

                commandMember = declaringType.Name + "_" + this.MemberInfo.GetName(this.MangleName);

                return commandMember.ToUpper();
            }
        }

        public string CommandText
        {
            get
            {
                string commandText;
                var declaringType = this.MemberInfo.GetOwningType(this.Type);

                commandText = declaringType.Name.ToLower() + "_" + this.MemberInfo.GetName(this.MangleName).ToLower();

                return commandText.ToLower();
            }
        }

        public string ReturnLambdaVariable
        {
            get
            {
                return this.ReturnVariable[0].ToString();
            }
        }

        public string ReturnVariable
        {
            get
            {
                Type returnType;
                string returnVariable;

                switch (this.MemberInfo)
                {
                    case MethodInfo methodInfo:

                        returnType = methodInfo.ReturnType;

                        if (this.IsCollectionReturn)
                        {
                            returnVariable = returnType.GetGenericArguments().First().Name.Pluralize().ToCamelCase();
                        }
                        else if (this.IsArrayReturn)
                        {
                            returnVariable = returnType.GetElementType().Name.Pluralize().ToCamelCase();
                        }
                        else
                        {
                            returnVariable = returnType.Name.ToCamelCase();
                        }

                        break;

                    case PropertyInfo propertyInfo:

                        returnType = propertyInfo.PropertyType;

                        if (this.IsCollectionReturn)
                        {
                            returnVariable = returnType.GetGenericArguments().First().Name.Pluralize().ToCamelCase();
                        }
                        else if (this.IsArrayReturn)
                        {
                            returnVariable = returnType.GetElementType().Name.Pluralize().ToCamelCase();
                        }
                        else
                        {
                            returnVariable = returnType.Name.ToCamelCase();
                        }

                        break;

                    default:
                        DebugUtils.Break();
                        returnVariable = null;
                        break;
                }

                return returnVariable;
            }
        }

        public string ParentVariable
        {
            get
            {
                var owningType = this.MemberInfo.GetOwningType(this.Type);

                return owningType.Name.ToCamelCase();
            }
        }

        public bool ParentTypeIsAssembly
        {
            get
            {
                var owningType = this.MemberInfo.GetOwningType(this.Type);

                return owningType.Name == "Assembly";
            }
        }

        public string ParentTypeName
        {
            get
            {
                var owningType = this.MemberInfo.GetOwningType(this.Type);

                return owningType.Name;
            }
        }

        public Type ParentType
        {
            get
            {
                var owningType = this.MemberInfo.GetOwningType(this.Type);

                return owningType;
            }
        }

        public Type ReturnElementType
        {
            get
            {
                Type returnType;
                Type returnElementType;

                if (this.ReturnElementTypeOverride != null)
                {
                    return this.ReturnElementTypeOverride;
                }

                switch (this.MemberInfo)
                {
                    case MethodInfo methodInfo:

                        returnType = methodInfo.ReturnType;

                        if (this.IsCollectionReturn)
                        {
                            returnElementType = returnType.GetGenericArguments().First();
                        }
                        else if (this.IsArrayReturn)
                        {
                            returnElementType = returnType.GetElementType();
                        }
                        else
                        {
                            returnElementType = returnType;
                        }

                        break;

                    case PropertyInfo propertyInfo:

                        returnType = propertyInfo.PropertyType;

                        if (this.IsCollectionReturn)
                        {
                            returnElementType = returnType.GetGenericArguments().First();
                        }
                        else if (this.IsArrayReturn)
                        {
                            returnElementType = returnType.GetElementType();
                        }
                        else
                        {
                            returnElementType = returnType;
                        }

                        break;

                    default:
                        DebugUtils.Break();
                        returnElementType = null;
                        break;
                }

                return returnElementType;
            }
        }
        public string ReturnElementTypeText
        {
            get
            {
                Type returnType;
                string returnElementTypeText;

                if (this.ReturnElementTypeTextOverride != null)
                {
                    return this.ReturnElementTypeTextOverride;
                }

                switch (this.MemberInfo)
                {
                    case MethodInfo methodInfo:

                        returnType = methodInfo.ReturnType;

                        if (this.IsCollectionReturn)
                        {
                            returnElementTypeText = returnType.GetGenericArguments().First().Name;
                        }
                        else if (this.IsArrayReturn)
                        {
                            returnElementTypeText = returnType.GetElementType().Name;
                        }
                        else
                        {
                            returnElementTypeText = returnType.Name;
                        }

                        break;

                    case PropertyInfo propertyInfo:

                        returnType = propertyInfo.PropertyType;

                        if (this.IsCollectionReturn)
                        {
                            returnElementTypeText = returnType.GetGenericArguments().First().Name;
                        }
                        else if (this.IsArrayReturn)
                        {
                            returnElementTypeText = returnType.GetElementType().Name;
                        }
                        else
                        {
                            returnElementTypeText = returnType.Name;
                        }

                        break;

                    default:
                        DebugUtils.Break();
                        returnElementTypeText = null;
                        break;
                }

                return returnElementTypeText;
            }
        }

        public bool IsCollectionReturn
        {
            get
            {
                var isCollectionReturn = false;

                switch (this.MemberInfo)
                {
                    case MethodInfo methodInfo:
                        isCollectionReturn = methodInfo.ReturnType.IsGenericCollection();
                        break;
                    case PropertyInfo propertyInfo:
                        isCollectionReturn = propertyInfo.PropertyType.IsGenericCollection();
                        break;
                    default:
                        DebugUtils.Break();
                        isCollectionReturn = false;
                        break;
                }

                return isCollectionReturn;
            }
        }

        public bool IsArrayReturn
        {
            get
            {
                var isArrayReturn = false;

                switch (this.MemberInfo)
                {
                    case MethodInfo methodInfo:
                        isArrayReturn = methodInfo.ReturnType.IsArray;
                        break;
                    case PropertyInfo propertyInfo:
                        isArrayReturn = propertyInfo.PropertyType.IsArray;
                        break;
                    default:
                        DebugUtils.Break();
                        isArrayReturn = false;
                        break;
                }

                return isArrayReturn;
            }
        }

        public string ClientSignature
        {
            get
            {
                string signature;
                var declaringType = this.MemberInfo.GetOwningType(this.Type);

                switch (this.MemberInfo)
                {
                    case MethodInfo methodInfo:

                        signature = declaringType.Name + "_" + methodInfo.GetSignature(noReturnType: true);
                        signature = signature.RegexReplace(@"\(\)", "(string identifier)");
                        signature = signature.RegexReplace(@"\((?!string identifier)", "(string identifier, ");

                        break;

                    case PropertyInfo propertyInfo:

                        var getMethod = propertyInfo.GetMethod;

                        signature = declaringType.Name + "_" + getMethod.GetSignature(noReturnType: true);
                        signature = signature.RegexReplace(@"\(\)", "(string identifier)");
                        signature = signature.RegexReplace(@"\((?!string identifier)", "(string identifier, ");

                        break;

                    default:

                        DebugUtils.Break();
                        signature = null;
                        break;
                }

                return signature;
            }
        }

        public string ArgumentsList
        {
            get
            {
                string argumentList;
                var declaringType = this.MemberInfo.GetOwningType(this.Type);

                switch (this.MemberInfo)
                {
                    case MethodInfo methodInfo:

                        var arguments = new List<string>();

                        foreach (var parm in methodInfo.GetParameters())
                        {
                            if (parm.ParameterType == typeof(string))
                            {
                                arguments.Add(parm.Name.ToCamelCase());
                            }
                            else
                            {
                                arguments.Add(parm.Name.ToCamelCase() + ".ToString()");
                            }
                        }

                        argumentList = arguments.ToCommaDelimitedList();
                        break;

                    case PropertyInfo propertyInfo:

                        argumentList = string.Empty;
                        break;

                    default:

                        DebugUtils.Break();
                        argumentList = null;
                        break;
                }

                return argumentList;
            }
        }

        public string ClientCode
        {
            get
            {
                var owningType = this.MemberInfo.GetOwningType();
                var owningType2 = this.MemberInfo.GetOwningType(this.Type);
                var memberName = this.MemberInfo.Name;
                var isTypeReturn = false;
                var isArrayTypeReturn = false;
                var isGenericTypeReturn = false;
                Type returnType;
                string apiCall;
                string fieldName;
                List<string> arguments;

                switch (this.MemberInfo)
                {
                    case MethodInfo methodInfo:

                        returnType = methodInfo.ReturnType;

                        if (returnType == typeof(Type))
                        {
                            isTypeReturn = true;
                        }
                        else if (returnType.IsArrayOf<Type>())
                        {
                            isArrayTypeReturn = true;
                        }
                        else if (returnType.IsGenericCollectionOf<Type>())
                        {
                            isGenericTypeReturn = true;
                        }

                        fieldName = owningType2.Name.ToCamelCase();
                        arguments = new List<string>();

                        if (this.CacheResult)
                        {
                            return this.HandleCacheResult(this.MemberInfo, fieldName);
                        }

                        if (isTypeReturn)
                        {
                            DebugUtils.Break();
                            apiCall = null;
                        }
                        else if (isGenericTypeReturn)
                        {
                            DebugUtils.Break();
                            apiCall = null;
                        }
                        else if (isArrayTypeReturn)
                        {
                            apiCall = "return agent.MapTypes(() => agent." + owningType.Name + "_" + methodInfo.Name + "(";

                            if (owningType.Name == "Assembly")
                            {
                                arguments.Add("parentIdentifier");
                            }
                            else
                            {
                                arguments.Add("string.Format(\"{0}[@MetadataToken={1}]\", parentIdentifier, " + fieldName + ".MetadataToken.ToString())");
                            }

                            foreach (var parm in methodInfo.GetParameters())
                            {
                                arguments.Add(parm.Name.ToCamelCase());
                            }

                            apiCall += arguments.ToCommaDelimitedList();
                            apiCall += "));";
                        }
                        else
                        {
                            apiCall = "return agent." + owningType.Name + "_" + methodInfo.Name + "(";

                            if (owningType.Name == "Assembly")
                            {
                                arguments.Add("parentIdentifier");
                            }
                            else
                            {
                                arguments.Add("string.Format(\"{0}[@MetadataToken={1}]\", parentIdentifier, " + fieldName + ".MetadataToken.ToString())");
                            }

                            foreach (var parm in methodInfo.GetParameters())
                            {
                                arguments.Add(parm.Name.ToCamelCase());
                            }

                            apiCall += arguments.ToCommaDelimitedList();
                            apiCall += ");";
                        }

                        break;

                    case PropertyInfo propertyInfo:

                        var getMethod = propertyInfo.GetMethod;

                        returnType = propertyInfo.PropertyType;

                        if (returnType == typeof(Type))
                        {
                            isTypeReturn = true;
                            DebugUtils.Break();
                        }
                        else if (returnType.IsArrayOf<Type>())
                        {
                            isArrayTypeReturn = true;
                            DebugUtils.Break();
                        }
                        else if (returnType.IsGenericCollectionOf<Type>())
                        {
                            isGenericTypeReturn = true;
                            DebugUtils.Break();
                        }

                        fieldName = owningType.Name.ToCamelCase();

                        if (this.CacheResult)
                        {
                            return this.HandleCacheResult(this.MemberInfo, fieldName);
                        }

                        arguments = new List<string>();

                        apiCall = "return agent." + owningType.Name + "_" + getMethod.Name + "(";

                        if (owningType.Name == "Assembly")
                        {
                            arguments.Add("parentIdentifier");
                        }
                        else
                        {
                            arguments.Add("string.Format(\"{0}[@MetadataToken={1}]\", parentIdentifier, " + fieldName + ".MetadataToken.ToString())");
                        }

                        foreach (var parm in getMethod.GetParameters())
                        {
                            arguments.Add(parm.Name.ToCamelCase());
                        }

                        apiCall += arguments.ToCommaDelimitedList();
                        apiCall += ");";

                        break;

                    default:

                        DebugUtils.Break();
                        apiCall = null;
                        break;
                }

                return apiCall;
            }
        }

        private string HandleCacheResult(MemberInfo memberInfo, string fieldName)
        {
            var memberName = memberInfo.Name;

            if (this.ClientReturnType.IsEnum)
            {
              return string.Format("return EnumUtils.GetValue<{0}>({1}.{2}MemberEnum);", this.ClientReturnTypeName, fieldName, memberInfo.Name);
            }
            else if (this.ClientReturnType == typeof(object))
            {
              return string.Format("return {0}.{1}MemberObject;", fieldName, memberInfo.Name);
            }
            else if (this.ClientReturnType.IsScalar())
            {
                return string.Format("return {0}.{1}Member;", fieldName, memberInfo.Name);
            }
            else if (memberInfo.Name == "ArgumentType")
            {
                return string.Format("return {0}.{1}FullName;", fieldName, memberInfo.Name);
            }
            else if (memberInfo.Name == "MemberInfo")
            {
                return string.Format("return {0}.{1}Name;", fieldName, memberInfo.Name);
            }
            else if (memberInfo.Name == "TypedValue")
            {
                return string.Format("return {0}.{1}Text;", fieldName, memberInfo.Name);
            }
            else if (this.ClientReturnType.IsGenericCollection())
            {
                var itemType = this.ClientReturnType.GetGenericArguments()[0];

                if (itemType.IsScalar())
                {
                    DebugUtils.Break();
                }
                else
                {
                    DebugUtils.Break();
                }
            }
            else
            {
                DebugUtils.Break();
            }

            return null;
        }

        public string ServerGetArgumentsCode
        {
            get
            {
                var declaringType = this.MemberInfo.GetOwningType(this.Type);
                var builder = new StringBuilder();
                var x = 0;

                switch (this.MemberInfo)
                {
                    case MethodInfo methodInfo:

                        foreach (var parm in methodInfo.GetParameters())
                        {
                            var type = parm.ParameterType;
                            var fieldName = parm.Name;

                            if (type.IsEnum)
                            {
                                builder.AppendLineFormat("var {0} = {1}", fieldName, string.Format("EnumUtils.GetValue<{0}>(args[{1}]);", type, x));
                            }
                            else if (type.IsScalar())
                            {
                                builder.AppendLineFormat("var {0} = ({1}) {2}", fieldName, type.GetShortName(), string.Format("Convert.ChangeType(args[{0}], typeof({1}));", x, type.GetShortName()));
                            }
                            else if (type == typeof(Type))
                            {
                                builder.AppendLineFormat("var {0} = {1}", fieldName, string.Format("Type.GetType(args[{0}]);", x));
                            }
                            else
                            {
                                DebugUtils.Break();
                            }

                            x++;
                        }

                        break;

                    case PropertyInfo propertyInfo:
                        {
                            var type = propertyInfo.PropertyType;
                            var fieldName = "value";

                            if (type.IsEnum)
                            {
                                builder.AppendLineFormat("var {0} = {1}", fieldName, string.Format("EnumUtils.GetValue<{0}>(args[{1}]);", type, x));
                            }
                            else if (type.IsScalar())
                            {
                                builder.AppendLineFormat("var {0} = ({1}) {2}", fieldName, type.GetShortName(), string.Format("Convert.ChangeType(args[{0}], typeof({1}));", x, type.GetShortName()));
                            }
                            else if (type == typeof(Type))
                            {
                                builder.AppendLineFormat("var {0} = {1}", fieldName, string.Format("Type.GetType(args[{0}]);", x));
                            }
                            else
                            {
                                DebugUtils.Break();
                            }
                        }

                        break;

                    default:

                        DebugUtils.Break();
                        break;
                }

                return builder.ToString();
            }
        }

        public string ServerCallCode
        {
            get
            {
                var declaringType = this.MemberInfo.GetOwningType(this.Type);
                string serverCall;
                string fieldName;
                List<string> arguments;

                switch (this.MemberInfo)
                {
                    case MethodInfo methodInfo:

                        fieldName = declaringType.Name.ToCamelCase();
                        arguments = new List<string>();

                        if (!methodInfo.IsPublic)
                        {
                            var parms = methodInfo.GetParameters();

                            if (methodInfo.ReturnType.IsEnum)
                            {
                                serverCall = string.Format("CallProtectedMethod<{0}>(\"" + methodInfo.Name + "\"", "int");
                            }
                            else
                            {
                                serverCall = string.Format("CallProtectedMethod<{0}>(\"" + methodInfo.Name + "\"", methodInfo.ReturnType.GetShortName());
                            }

                            if (parms.Length > 0)
                            {
                                serverCall += ", ";

                                foreach (var parm in methodInfo.GetParameters())
                                {
                                    arguments.Add(parm.Name.ToCamelCase());
                                }

                                serverCall += arguments.ToCommaDelimitedList();
                            }

                            serverCall += ")";
                        }
                        else
                        {
                            if (methodInfo.Name == "GetCustomAttributes")
                            {
                                serverCall = "CustomAttributes.Cast<CustomAttributeData>()";
                            }
                            else
                            {
                                serverCall = methodInfo.Name + "(";

                                foreach (var parm in methodInfo.GetParameters())
                                {
                                    arguments.Add(parm.Name.ToCamelCase());
                                }

                                serverCall += arguments.ToCommaDelimitedList();
                                serverCall += ")";
                            }
                        }

                        break;

                    case PropertyInfo propertyInfo:

                        var getMethod = propertyInfo.GetMethod;

                        fieldName = declaringType.Name.ToCamelCase();
                        arguments = new List<string>();
                        serverCall = propertyInfo.Name;

                        break;

                    default:

                        DebugUtils.Break();
                        serverCall = null;
                        break;
                }

                return serverCall;
            }
        }
    }
}
