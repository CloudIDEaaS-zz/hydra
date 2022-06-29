// <startfile>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public class TestPreprocessors
    {
#if true
        // Test1
#if true
        // Test2
#endif        
        // Test3 
#endif
        // Test4
        
        public void Test()
        {
            // <startvariables readonly="true">

            string str = "Test";

            // </endvariables>

            str = "Test";

            // <custom>

            str = string.Empty;
            
            // </custom>
        }
    }
}

// </startfile>
