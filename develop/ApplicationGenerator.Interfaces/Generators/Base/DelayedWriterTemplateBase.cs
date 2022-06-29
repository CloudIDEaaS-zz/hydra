// file:	Generators\Base\DelayedWriterTemplateBase.cs
//
// summary:	Implements the delayed writer template base class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Generators.Base
{
    /// <summary>   A delayed writer template base. </summary>
    ///
    /// <remarks>   Ken, 10/16/2020. </remarks>

    public abstract class DelayedWriterTemplateBase : TemplateBase
    {
        /// <summary>   Gets the writer actions. </summary>
        ///
        /// <value> The writer actions. </value>

        public Dictionary<int, List<WriterAction>> WriterActions { get; }

        /// <summary>   Gets or sets the current node kind. </summary>
        ///
        /// <value> The current node kind. </value>

        public int CurrentNodeKind { get; set; }

        /// <summary>   Gets or sets the current node. </summary>
        ///
        /// <value> The current node. </value>

        public ISemanticTreeBaseNode CurrentNode { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Ken, 10/16/2020. </remarks>

        public DelayedWriterTemplateBase()
        {
            this.WriterActions = new Dictionary<int, List<WriterAction>>();
        }

        /// <summary>   Writes all. </summary>
        ///
        /// <remarks>   Ken, 10/16/2020. </remarks>
        ///
        /// <typeparam name="TKind">    Type of the kind. </typeparam>
        /// <param name="kindFlags">    The kind flags. </param>

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

        /// <summary>   Writes. </summary>
        ///
        /// <remarks>   Ken, 10/16/2020. </remarks>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public override void Write(string format, params object[] args)
        {
            var action = new Action(() =>
            {
                base.Write(format, args);
            });

            this.WriterActions.AddToDictionaryListCreateIfNotExist(this.CurrentNodeKind, new WriterAction(this.CurrentNode, action));
        }

        /// <summary>   Writes. </summary>
        ///
        /// <remarks>   Ken, 10/16/2020. </remarks>
        ///
        /// <param name="textToAppend"> The text to append. </param>

        public override void Write(string textToAppend)
        {
            var action = new Action(() =>
            {
                base.Write(textToAppend);
            });

            this.WriterActions.AddToDictionaryListCreateIfNotExist(this.CurrentNodeKind, new WriterAction(this.CurrentNode, action));
        }

        /// <summary>   Writes a line. </summary>
        ///
        /// <remarks>   Ken, 10/16/2020. </remarks>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public override void WriteLine(string format, params object[] args)
        {
            var action = new Action(() =>
            {
                base.WriteLine(format, args);
            });

            this.WriterActions.AddToDictionaryListCreateIfNotExist(this.CurrentNodeKind, new WriterAction(this.CurrentNode, action));
        }

        /// <summary>   Writes a line. </summary>
        ///
        /// <remarks>   Ken, 10/16/2020. </remarks>
        ///
        /// <param name="textToAppend"> The text to append. </param>

        public override void WriteLine(string textToAppend)
        {
            var action = new Action(() =>
            {
                base.WriteLine(textToAppend);
            });

            this.WriterActions.AddToDictionaryListCreateIfNotExist(this.CurrentNodeKind, new WriterAction(this.CurrentNode, action));
        }

        /// <summary>   Writes. </summary>
        ///
        /// <remarks>   Ken, 10/16/2020. </remarks>
        ///
        /// <param name="indent">   The indent. </param>
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public override void Write(int indent, string format, params object[] args)
        {
            var action = new Action(() =>
            {
                this.Write(SPACE.Repeat(indent) + string.Format(format, args));
            });

            this.WriterActions.AddToDictionaryListCreateIfNotExist(this.CurrentNodeKind, new WriterAction(this.CurrentNode, action));
        }

        /// <summary>   Writes. </summary>
        ///
        /// <remarks>   Ken, 10/16/2020. </remarks>
        ///
        /// <param name="indent">       The indent. </param>
        /// <param name="textToAppend"> The text to append. </param>

        public override void Write(int indent, string textToAppend)
        {
            var action = new Action(() =>
            {
                this.Write(SPACE.Repeat(indent) + textToAppend);
            });

            this.WriterActions.AddToDictionaryListCreateIfNotExist(this.CurrentNodeKind, new WriterAction(this.CurrentNode, action));
        }

        /// <summary>   Writes a line. </summary>
        ///
        /// <remarks>   Ken, 10/16/2020. </remarks>
        ///
        /// <param name="indent">       The indent. </param>
        /// <param name="textToAppend"> The text to append. </param>

        public override void WriteLine(int indent, string textToAppend)
        {
            var action = new Action(() =>
            {
                this.WriteLine(SPACE.Repeat(indent) + textToAppend);
            });

            this.WriterActions.AddToDictionaryListCreateIfNotExist(this.CurrentNodeKind, new WriterAction(this.CurrentNode, action));
        }

        /// <summary>   Writes a line. </summary>
        ///
        /// <remarks>   Ken, 10/16/2020. </remarks>
        ///
        /// <param name="indent">   The indent. </param>
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

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
