
namespace SassParser
{
    public interface IMediaRule : IConditionRule
    {
        MediaList Media { get; }
    }
}