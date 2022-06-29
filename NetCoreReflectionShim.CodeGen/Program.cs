using System;
using System.Reflection;
using System.Linq;
using Utils;
using NetCoreReflectionShim.CodeGen.Generators;
using System.Collections.Generic;
using AbstraX;

namespace NetCoreReflectionShim.CodeGen
{
    public class Program
    {
        private static bool TEST = true;

        public static void Main(string[] args)
        {
            var assemblyType = typeof(Assembly);
            var memberInfoType = typeof(MemberInfo);
            var methodBaseInfoType = typeof(MethodBase);
            var valueType = typeof(ValueType);
            //var appResources = typeof(AppResources);
            //var queryInfo = typeof(QueryInfo);

            if (TEST)
            {
                DoTests();
            }

            ReflectionGenerator.ReflectMember += ReflectionCallbacks.JsonReflectionGenerator_ReflectMember;
            ReflectionGenerator.GetApiBody += ReflectionCallbacks.ReflectionGenerator_GetApiBody;

            ReflectionGenerator.GenerateClass(assemblyType);
            ReflectionGenerator.GenerateClass(memberInfoType);
            ReflectionGenerator.GenerateClass(methodBaseInfoType);
            ReflectionGenerator.GenerateClass(valueType, true);
            //ReflectionGenerator.GenerateClass(appResources);
            //ReflectionGenerator.GenerateClass(queryInfo, true);

            ReflectionGenerator.GenerateApi(ReflectionCallbacks.APIMembers.Values.ToList());

            Console.WriteLine("Complete! Press any key to exit.");
            Console.ReadKey();
        }

        private static void DoTests()
        {
            //var assembly = Assembly.GetEntryAssembly();
            //var types = assembly.GetTypes().OrderBy(n => n.Name).Select(t => ClientMapper.Map(t)).ToList();
            Type type;
            ParameterInfo parameterInfo;
            PropertyInfo propertyInfo;
            MemberInfo memberInfo;
            System.Reflection.CustomAttributeData customAttributeData;
            ConstructorInfo constructorInfo;
            System.Reflection.CustomAttributeTypedArgument customAttributeTypedArgument;

            //foreach (var type in types)
            //{
            //    var json = JsonExtensions.ToJsonText(type);
            //    var convertedType = JsonExtensions.ReadJson<TypeJson>(json);

            //    Console.WriteLine(convertedType.FullName);
            //}
        }
    }
}
