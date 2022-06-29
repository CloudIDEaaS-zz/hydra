﻿namespace SassParser
{
    public interface IRule : IStylesheetNode
    {
        RuleType Type { get; }
        string Text { get; set; }
        IRule Parent { get; }
        Stylesheet Owner { get; }
    }
}