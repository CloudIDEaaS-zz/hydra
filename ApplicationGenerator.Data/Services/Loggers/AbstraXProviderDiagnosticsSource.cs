using System.Diagnostics;

namespace ApplicationGenerator.Data
{
    public class AbstraXProviderDiagnosticsSource : DiagnosticSource
    {
        public override bool IsEnabled(string name)
        {
            return true;
        }

        public override void Write(string name, object value)
        {
        }
    }
}