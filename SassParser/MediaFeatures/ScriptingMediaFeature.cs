
namespace SassParser
{
    internal sealed class ScriptingMediaFeature : MediaFeature
    {
        private static readonly IValueConverter TheConverter = Map.ScriptingStates.ToConverter();

        public ScriptingMediaFeature(Token token) : base(FeatureNames.Scripting, token)
        {
        }

        internal override IValueConverter Converter => TheConverter;
    }
}