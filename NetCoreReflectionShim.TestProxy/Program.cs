using AbstraX;
using Utils;

namespace NetCoreReflectionShim.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TestRuntimeProxy();
        }

        private static void TestRuntimeProxy()
        {
            IRuntime runtimeObj;
            var proxy = new RuntimeProxy();
            string name;

            runtimeObj = RuntimeProxyExtensions.WrapInstance<IRuntime>(proxy, string.Empty);

            runtimeObj.Test("arg1", 123, typeof(object));
            runtimeObj.Name = "Hello";
            name = runtimeObj.Name;
        }
    }
}
  