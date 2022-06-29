namespace SassParser
{
    public enum UnicodeMode : byte
    {
        Normal,
        Embed,
        Isolate,
        BidirectionalOverride,
        IsolateOverride,
        Plaintext
    }
}