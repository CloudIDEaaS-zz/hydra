﻿namespace SassParser
{
    public interface IGroupingRule : IRule, IRuleCreator
    {
        IRuleList Rules { get; }
        int Insert(string rule, int index);
        void RemoveAt(int index);
    }
}