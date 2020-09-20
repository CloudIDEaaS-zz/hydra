using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.TextObjectModel
{
    public interface ITextDocument
    {
        string Name { get; }
        ITextSelection Selection { get; }
        int StoryCount { get; }
        ITextStoryRanges StoryRanges { get; }
        bool Saved { set; }
        float DefaultTabStop { set; }
        void New();
        void Open();
        void Save();
        int Freeze();
        int Unfreeze();
        void BeginEditCollection();
        void EndEditCollection();
        int Undo(int count);
        int Redo(int count);
        ITextRange Range(int start, int end);
        ITextRange RangeFromPoint(int x, int y);
        void ApplyRange(ITextRange range);
    }
}
