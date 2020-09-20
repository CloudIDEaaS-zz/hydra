using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Linq;
using CodeInterfaces.AssemblyInterfaces;
using Utils;

namespace CodeInterfaces
{
    public static class ExtensionMethods
    {
        public static string GetPropertyName(this IGetSetProperty property)
        {
            var method = property.Method;

            if (method.IsGetter())
            {
                return method.Name.Remove(0, 3).ProperCase();
            }
            else if (method.IsSetter())
            {
                return method.Name.Remove(0, 3).ProperCase();
            }
            else
            {
                throw new Exception("Method property must be a getter or setter");
            }
        }

        public static bool IsGetter(this MethodInfo method)
        {
            if (method.Name.StartsWith("get") && method.ReturnType != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsSetter(this MethodInfo method)
        {
            if (method.Name.StartsWith("set") && method.ReturnType == null && method.GetParameters().Length == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Type GetGetterSetterType(this MethodInfo method)
        {
            return method.ReturnType != null ? method.ReturnType : method.GetParameters().First().ParameterType;
        }

        public static string GenerateTypeString(this BaseType type)
        {
            if (type.GenericArguments.Length > 0)
            {
                var builder = new StringBuilder();
                var name = type.Name;
                var index = name.IndexOf("`");

                if (type.IsScalarType)
                {
                    name = type.Name.CamelCase();
                }

                if (index != -1)
                {
                    name = name.Remove(index);
                }

                builder.Append(name);
                builder.Append(GenerateGenericArgsString(type.GenericArguments));

                return builder.ToString();
            }
            else
            {
                return type.Name;
            }
        }

        public static string GenerateTypeString(this System.Type type)
        {
            if (type.IsGenericType)
            {
                var builder = new StringBuilder();
                var name = type.Name;
                var index = name.IndexOf("`");

                if (type.IsPrimitive)
                {
                    name = type.Name.CamelCase();
                }

                if (index != -1)
                {
                    name = name.Remove(index);
                }

                builder.Append(name);
                builder.Append(GenerateGenericArgsString(type.GetGenericArguments()));

                return builder.ToString();
            }
            else
            {
                return type.Name;
            }
        }

        public static string GenerateGenericArgsString(BaseType[] arguments)
        {
            var builder = new StringBuilder("<");

            var hasComma = false;

            foreach (BaseType argumentType in arguments)
            {
                if (argumentType.GenericArguments.Length > 0)
                {
                    builder.Append(GenerateGenericArgsString(argumentType.GetGenericArguments()));
                }
                else
                {
                    var name = argumentType.Name;

                    if (argumentType.IsScalarType)
                    {
                        name = argumentType.Name.CamelCase();
                    }

                    builder.Append(name);
                }

                builder.Append(", ");
                hasComma = true;
            }

            if (hasComma)
            {
                builder.Remove(builder.Length - 2, 2);
            }

            builder.Append(">");

            return builder.ToString();
        }

        public static string GenerateGenericArgsString(System.Type[] arguments)
        {
            var builder = new StringBuilder("<");

            var hasComma = false;

            foreach (System.Type argumentType in arguments)
            {
                if (argumentType.IsGenericType)
                {
                    builder.Append(GenerateGenericArgsString(argumentType.GetGenericArguments()));
                }
                else
                {
                    var name = argumentType.Name;

                    if (argumentType.IsPrimitive)
                    {
                        name = argumentType.Name.CamelCase();
                    }

                    builder.Append(name);
                }

                builder.Append(", ");
                hasComma = true;
            }

            if (hasComma)
            {
                builder.Remove(builder.Length - 2, 2);
            }

            builder.Append(">");

            return builder.ToString();
        }

        public static string GenerateParmString(ParameterInfo[] parameters)
        {
            var builder = new StringBuilder("(");
            var hasComma = false;

            foreach (ParameterInfo parameter in parameters)
            {
                if (parameter.ParameterType.IsByRef)
                {
                    builder.Append("ref ");
                }
                else if (parameter.IsOut)
                {
                    builder.Append("out ");
                }

                builder.Append(parameter.ParameterType.Name);

                if (parameter.ParameterType.IsArray)
                {
                    builder.Append("[]");
                }

                if (parameter.ParameterType.IsGenericType)
                {
                    builder.Append(GenerateGenericArgsString(parameter.ParameterType.GetGenericArguments()));
                }

                if (parameter.IsOptional)
                {
                    builder.Append(" = " + parameter.DefaultValue.ToString());
                }

                builder.Append(", ");
                hasComma = true;
            }

            if (hasComma)
            {
                builder.Remove(builder.Length - 2, 2);
            }

            builder.Append(")");

            return builder.ToString();
        }

        public static string GenerateName(this IOperatorOperation operatorOperation)
        {
            //var method = operatorOperation.Method;
            //var name = method.Name;

            //return name + GenerateParmString(method.GetParameters());

            throw new NotImplementedException();
        }

        public static string GenerateName(this IEventOperation eventOperation)
        {
            //var method = eventOperation.Method;
            //var name = method.Name;

            //return name + GenerateParmString(method.GetParameters());

            throw new NotImplementedException();
        }

        //public static string GenerateName(this DelegateOperation delegateOperation)
        //{
        //    var method = delegateOperation.Method;
        //    var name = method.Name;

        //    return name + GenerateParmString(method.GetParameters());
        //}

        public static string GenerateName(this IGetSetPropertyAttribute propertyAttribute)
        {
            var method = propertyAttribute.Method;
            var name = method.Name;

            return name + GenerateParmString(method.GetParameters());
        }

        public static string GenerateName(this IGetSetPropertyElement propertyElement)
        {
            var method = propertyElement.Method;
            var name = method.Name;

            return name + GenerateParmString(method.GetParameters());
        }

        public static string GenerateName(this MethodInfo method)
        {
            var name = method.Name;

            return name + GenerateParmString(method.GetParameters());
        }

        public static string GenerateName(this ConstructorInfo constructor)
        {
            var name = constructor.Name;

            return name + GenerateParmString(constructor.GetParameters());
        }

        public static string GenerateName(this IMethodOperation methodOperation)
        {
            var method = methodOperation.Method;
            var name = method.Name;

            return name + GenerateParmString(method.GetParameters());
        }

        public static string GenerateSignature(this IMethodOperation methodOperation)
        {
            var method = methodOperation.Method;
            var returnType = method.ReturnType != null ? method.ReturnType.Name : "void";
            var name = returnType + " " + method.Name;

            return name + GenerateParmStringSignature(method.GetParameters());
        }

        public static string GenerateParmsVariableList(this IMethodOperation methodOperation)
        {
            var method = methodOperation.Method;
            var variables = method.GetParameters().Select(p => p.ParameterType.Name.CamelCase());
            var list = variables.ToCommaDelimitedList();

            return list;
        }

        private static string GenerateParmStringSignature(ParameterInfo[] parameters)
        {
            var builder = new StringBuilder("(");
            var hasComma = false;

            foreach (ParameterInfo parameter in parameters)
            {
                if (parameter.ParameterType.IsByRef)
                {
                    builder.Append("ref ");
                }
                else if (parameter.IsOut)
                {
                    builder.Append("out ");
                }

                builder.Append(parameter.ParameterType.Name);

                if (parameter.ParameterType.IsArray)
                {
                    builder.Append("[]");
                }

                if (parameter.ParameterType.IsGenericType)
                {
                    builder.Append(GenerateGenericArgsString(parameter.ParameterType.GetGenericArguments()));
                }

                builder.Append(" " + parameter.ParameterType.Name.CamelCase());

                if (parameter.IsOptional)
                {
                    builder.Append(" = " + parameter.DefaultValue.ToString());
                }

                builder.Append(", ");
                hasComma = true;
            }

            if (hasComma)
            {
                builder.Remove(builder.Length - 2, 2);
            }

            builder.Append(")");

            return builder.ToString();
        }

        public static string GenerateName(this IConstructorOperation constructorOperation)
        {
            var constructor = constructorOperation.Constructor;
            var name = constructor.Name;

            return name + GenerateParmString(constructor.GetParameters());
        }
    }
}
