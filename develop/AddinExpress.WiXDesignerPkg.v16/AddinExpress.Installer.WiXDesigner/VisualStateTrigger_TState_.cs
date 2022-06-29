using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VisualStateTrigger<TState> : IEquatable<VisualStateTrigger<TState>>
	{
		public AnchorStyles Anchor
		{
			get;
			set;
		}

		public Rectangle Bounds
		{
			get;
			set;
		}

		public TState State
		{
			get;
			private set;
		}

		public VisualStateTriggerTypes Type
		{
			get;
			private set;
		}

		public VisualStateTrigger(VisualStateTriggerTypes type, TState state, Rectangle bounds = null, AnchorStyles anchor = 5)
		{
			this.Type = type;
			this.State = state;
			this.Bounds = bounds;
			this.Anchor = anchor;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is BufferedPaintTransition<TState>))
			{
				return this.Equals(obj);
			}
			return ((IEquatable<VisualStateTrigger<TState>>)this).Equals((VisualStateTrigger<TState>)obj);
		}

		public override int GetHashCode()
		{
			return this.Type.GetHashCode() ^ (this.State ?? 0).GetHashCode();
		}

		bool System.IEquatable<AddinExpress.Installer.WiXDesigner.VisualStateTrigger<TState>>.Equals(VisualStateTrigger<TState> other)
		{
			if (this.Type != other.Type)
			{
				return false;
			}
			return object.Equals(this.State, other.State);
		}
	}
}