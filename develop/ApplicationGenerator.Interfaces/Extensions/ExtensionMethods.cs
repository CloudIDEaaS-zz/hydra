// file:	Extensions\ExtensionMethods.cs
//
// summary:	Implements the extension methods class

using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Linq;
using AbstraX.ServerInterfaces;
using Utils;
using EntityProvider.Web.Entities;
using AbstraX.DataAnnotations;
using AbstraX.AssemblyInterfaces;
using System.Collections.Generic;
using System.IO;
using AbstraX.Models.Interfaces;

namespace AbstraX
{
    /// <summary>   An extension methods. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

    public static class ExtensionMethods
    {
        /// <summary>
        /// A Dictionary&lt;string,string&gt; extension method that queries if a given peer exists.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="peerDependencies"> The peerDependencies to act on. </param>
        /// <param name="nodeModules">      The node modules. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool PeerExists(this Dictionary<string, string> peerDependencies, NpmNodeModules nodeModules)
        {
            foreach (var peerDependencyPair in peerDependencies)
            {
                var name = peerDependencyPair.Key;
                NpmVersion version = peerDependencyPair.Value;

                foreach (var package in nodeModules.LoadedPackages.Where(p => p.Name == name))
                {
                    if (version.Matches(package.Version))
                    {
                        return true;
                    }
                    else if (version < package.Version)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// A Dictionary&lt;string,string&gt; extension method that all peers exist.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="peerDependencies"> The peerDependencies to act on. </param>
        /// <param name="nodeModules">      The node modules. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool AllPeersExist(this Dictionary<string, string> peerDependencies, NpmNodeModules nodeModules)
        {
            if (peerDependencies.Count == 0)
            {
                return false;
            }
            else
            {
                foreach (var peerDependencyPair in peerDependencies)
                {
                    var name = peerDependencyPair.Key;
                    NpmVersion version = peerDependencyPair.Value;

                    foreach (var package in nodeModules.LoadedPackages.Where(p => p.Name == name))
                    {
                        if (!version.Matches(package.Version))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The IEntityObjectWithFacets extension method that query if 'entityWithFacets' has navigation
        /// or user interface.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="entityWithFacets"> The entityWithFacets to act on. </param>
        ///
        /// <returns>   True if navigation or user interface, false if not. </returns>

        public static bool HasNavigationOrUI(this IEntityObjectWithFacets entityWithFacets)
        {
            var facets = entityWithFacets.Facets;
            var hasNavigationOrUI = facets.Any(f => f.Attribute is UINavigationAttribute || f.Attribute is UIAttribute);

            if (entityWithFacets is IEntityWithOptionalFacets)
            {
                var entityWithOptionalFacets = (IEntityWithOptionalFacets)entityWithFacets;

                if (entityWithOptionalFacets.FollowWithout)
                {
                    hasNavigationOrUI = true;
                }
            }

            return hasNavigationOrUI;
        }

        /// <summary>
        /// The IEntityObjectWithFacets extension method that determine if we can follow without.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="entityWithFacets"> The entityWithFacets to act on. </param>
        ///
        /// <returns>   True if we can follow without, false if not. </returns>

        public static bool CanFollowWithout(this IEntityObjectWithFacets entityWithFacets)
        {
            var facets = entityWithFacets.Facets;
            var hasNavigationOrUI = facets.Any(f => f.Attribute is UINavigationAttribute || f.Attribute is UIAttribute);

            if (entityWithFacets is IEntityWithOptionalFacets)
            {
                var entityWithOptionalFacets = (IEntityWithOptionalFacets)entityWithFacets;

                if (entityWithOptionalFacets.FollowWithout)
                {
                    hasNavigationOrUI = true;
                }
            }

            return hasNavigationOrUI;
        }

        /// <summary>
        /// The IEntityObjectWithFacets extension method that query if 'entityWithFacets' has user
        /// interface.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="entityWithFacets"> The entityWithFacets to act on. </param>
        ///
        /// <returns>   True if user interface, false if not. </returns>

        public static bool HasUI(this IEntityObjectWithFacets entityWithFacets)
        {
            var facets = entityWithFacets.Facets;

            return facets.Any(f => f.Attribute is UIAttribute);
        }

        /// <summary>   An IGetSetProperty extension method that gets property name. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="property"> The property to act on. </param>
        ///
        /// <returns>   The property name. </returns>

        public static string GetPropertyName(this IGetSetProperty property)
        {
            var method = property.Method;

            if (method.IsGetter())
            {
                return method.Name.Remove(0, 3).ToTitleCase();
            }
            else if (method.IsSetter())
            {
                return method.Name.Remove(0, 3).ToTitleCase();
            }
            else
            {
                throw new Exception("Method property must be a getter or setter");
            }
        }

        /// <summary>   A MethodInfo extension method that query if 'method' is getter. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="method">   The method to act on. </param>
        ///
        /// <returns>   True if getter, false if not. </returns>

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

        /// <summary>   A MethodInfo extension method that query if 'method' is setter. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="method">   The method to act on. </param>
        ///
        /// <returns>   True if setter, false if not. </returns>

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

        /// <summary>   A MethodInfo extension method that gets getter setter type. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="method">   The method to act on. </param>
        ///
        /// <returns>   The getter setter type. </returns>

        public static Type GetGetterSetterType(this MethodInfo method)
        {
            return method.ReturnType != null ? method.ReturnType : method.GetParameters().First().ParameterType;
        }

        /// <summary>   A System.Type extension method that generates a type string. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="type"> The type. </param>
        ///
        /// <returns>   The type string. </returns>

        public static string GenerateTypeString(this BaseType type)
        {
            if (type.GenericArguments.Length > 0)
            {
                var builder = new StringBuilder();
                var name = type.Name;
                var index = name.IndexOf("`");

                if (type.IsScalarType)
                {
                    name = type.Name.ToCamelCase();
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

        /// <summary>   A System.Type extension method that generates a type string. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="type"> The type. </param>
        ///
        /// <returns>   The type string. </returns>

        public static string GenerateTypeString(this System.Type type)
        {
            if (type.IsGenericType)
            {
                var builder = new StringBuilder();
                var name = type.Name;
                var index = name.IndexOf("`");

                if (type.IsPrimitive)
                {
                    name = type.Name.ToCamelCase();
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

        /// <summary>   Generates a generic arguments string. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="arguments">    The arguments. </param>
        ///
        /// <returns>   The generic arguments string. </returns>

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
                        name = argumentType.Name.ToCamelCase();
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

        /// <summary>   Generates a generic arguments string. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="arguments">    The arguments. </param>
        ///
        /// <returns>   The generic arguments string. </returns>

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
                        name = argumentType.Name.ToCamelCase();
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

        /// <summary>   Generates a parameter string. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="parameters">   Options for controlling the operation. </param>
        ///
        /// <returns>   The parameter string. </returns>

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

        /// <summary>   An IConstructorOperation extension method that generates a name. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="operatorOperation">    The operatorOperation to act on. </param>
        ///
        /// <returns>   The name. </returns>

        public static string GenerateName(this IOperatorOperation operatorOperation)
        {
            //var method = operatorOperation.Method;
            //var name = method.Name;

            //return name + GenerateParmString(method.GetParameters());

            throw new NotImplementedException();
        }

        /// <summary>   An IConstructorOperation extension method that generates a name. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="eventOperation">   The eventOperation to act on. </param>
        ///
        /// <returns>   The name. </returns>

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

        /// <summary>   An IConstructorOperation extension method that generates a name. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="propertyAttribute">    The propertyAttribute to act on. </param>
        ///
        /// <returns>   The name. </returns>

        public static string GenerateName(this IGetSetPropertyAttribute propertyAttribute)
        {
            var method = propertyAttribute.Method;
            var name = method.Name;

            return name + GenerateParmString(method.GetParameters());
        }

        /// <summary>   An IConstructorOperation extension method that generates a name. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="propertyElement">  The propertyElement to act on. </param>
        ///
        /// <returns>   The name. </returns>

        public static string GenerateName(this IGetSetPropertyElement propertyElement)
        {
            var method = propertyElement.Method;
            var name = method.Name;

            return name + GenerateParmString(method.GetParameters());
        }

        /// <summary>   An IConstructorOperation extension method that generates a name. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="method">   The method to act on. </param>
        ///
        /// <returns>   The name. </returns>

        public static string GenerateName(this MethodInfo method)
        {
            var name = method.Name;

            return name + GenerateParmString(method.GetParameters());
        }

        /// <summary>   An IConstructorOperation extension method that generates a name. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="constructor">  The constructor to act on. </param>
        ///
        /// <returns>   The name. </returns>

        public static string GenerateName(this ConstructorInfo constructor)
        {
            var name = constructor.Name;

            return name + GenerateParmString(constructor.GetParameters());
        }

        /// <summary>   An IConstructorOperation extension method that generates a name. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="methodOperation">  The methodOperation to act on. </param>
        ///
        /// <returns>   The name. </returns>

        public static string GenerateName(this IMethodOperation methodOperation)
        {
            var method = methodOperation.Method;
            var name = method.Name;

            return name + GenerateParmString(method.GetParameters());
        }

        /// <summary>   An IMethodOperation extension method that generates a signature. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="methodOperation">  The methodOperation to act on. </param>
        ///
        /// <returns>   The signature. </returns>

        public static string GenerateSignature(this IMethodOperation methodOperation)
        {
            var method = methodOperation.Method;
            var returnType = method.ReturnType != null ? method.ReturnType.Name : "void";
            var name = returnType + " " + method.Name;

            return name + GenerateParmStringSignature(method.GetParameters());
        }

        /// <summary>
        /// An IMethodOperation extension method that generates the parameters variable list.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="methodOperation">  The methodOperation to act on. </param>
        ///
        /// <returns>   The parameters variable list. </returns>

        public static string GenerateParmsVariableList(this IMethodOperation methodOperation)
        {
            var method = methodOperation.Method;
            var variables = method.GetParameters().Select(p => p.ParameterType.Name.ToCamelCase());
            var list = variables.ToCommaDelimitedList();

            return list;
        }

        /// <summary>   Generates a parameter string signature. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="parameters">   Options for controlling the operation. </param>
        ///
        /// <returns>   The parameter string signature. </returns>

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

                builder.Append(" " + parameter.ParameterType.Name.ToCamelCase());

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

        /// <summary>   An IConstructorOperation extension method that generates a name. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="constructorOperation"> The constructorOperation to act on. </param>
        ///
        /// <returns>   The name. </returns>

        public static string GenerateName(this IConstructorOperation constructorOperation)
        {
            var constructor = constructorOperation.Constructor;
            var name = constructor.Name;

            return name + GenerateParmString(constructor.GetParameters());
        }
    }
}
