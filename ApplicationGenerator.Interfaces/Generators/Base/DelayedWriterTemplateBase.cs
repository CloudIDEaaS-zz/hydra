using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Generators.Base
{
    public abstract class DelayedWriterTemplateBase : TemplateBase
    {
        public Dictionary<int, List<WriterAction>> WriterActions { get; }
        public int CurrentNodeKind { get; set; }
        public ISemanticTreeBaseNode CurrentNode { get; set; }

        public DelayedWriterTemplateBase()
        {
            this.WriterActions = new Dictionary<int, List<WriterAction>>();
        }

        public void WriteAll<TKind>(TKind kindFlags) where TKind : struct, IComparable, IConvertible, IFormattable
        {
            var kinds = EnumUtils.GetValues<TKind>(kindFlags);
            var writerActions = new List<WriterAction>();

            foreach (var kind in kinds)
            {
                var kindInt = (int)(object)kind;

                if (this.WriterActions.ContainsKey(kindInt))
                {
                    var writerActionsByKind = this.WriterActions[kindInt];

                    writerActions.AddRange(writerActionsByKind);
                }
            }

            foreach (var writerAction in writerActions.OrderBy(a => a.Counter))
            {
                writerAction.Action();
            }
        }

        public override void Write(string format, params object[] args)
        {
            var action = new Action(() =>
            {
                base.Write(format, args);
            });

            this.WriterActions.AddToDictionaryListCreateIfNotExist(this.CurrentNodeKind, new WriterAction(this.CurrentNode, action));
        }

        public override void Write(string textToAppend)
        {
            var action = new Action(() =>
            {
                base.Write(textToAppend);
            });

            this.WriterActions.AddToDictionaryListCreateIfNotExist(this.CurrentNodeKind, new WriterAction(this.CurrentNode, action));
        }

        public override void WriteLine(string format, params object[] args)
        {
            var action = new Action(() =>
            {
                base.WriteLine(format, args);
            });

            this.WriterActions.AddToDictionaryListCreateIfNotExist(this.CurrentNodeKind, new WriterAction(this.CurrentNode, action));
        }

        public override void WriteLine(string textToAppend)
        {
            var action = new Action(() =>
            {
                base.WriteLine(textToAppend);
            });

            this.WriterActions.AddToDictionaryListCreateIfNotExist(this.CurrentNodeKind, new WriterAction(this.CurrentNode, action));
        }

        public override void Write(int indent, string format, params object[] args)
        {
            var action = new Action(() =>
            {
                this.Write(SPACE.Repeat(indent) + string.Format(format, args));
            });

            this.WriterActions.AddToDictionaryListCreateIfNotExist(this.CurrentNodeKind, new WriterAction(this.CurrentNode, action));
        }

        public override void Write(int indent, string textToAppend)
        {
            var action = new Action(() =>
            {
                this.Write(SPACE.Repeat(indent) + textToAppend);
            });

            this.WriterActions.AddToDictionaryListCreateIfNotExist(this.CurrentNodeKind, new WriterAction(this.CurrentNode, action));
        }

        public override void WriteLine(int indent, string textToAppend)
        {
            var action = new Action(() =>
            {
                this.WriteLine(SPACE.Repeat(indent) + textToAppend);
            });

            this.WriterActions.AddToDictionaryListCreateIfNotExist(this.CurrentNodeKind, new WriterAction(this.CurrentNode, action));
        }

        public override void WriteLine(int indent, string format, params object[] args)
        {
            var action = new Action(() =>
            {
                this.WriteLine(SPACE.Repeat(indent) + string.Format(format, args));
            });

            this.WriterActions.AddToDictionaryListCreateIfNotExist(this.CurrentNodeKind, new WriterAction(this.CurrentNode, action));
        }
    }
}
