using System;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class BufferedPaintTransition<TState> : IEquatable<BufferedPaintTransition<TState>>
	{
		public int Duration
		{
			get;
			set;
		}

		public TState FromState
		{
			get;
			private set;
		}

		public TState ToState
		{
			get;
			private set;
		}

		public BufferedPaintTransition(TState fromState, TState toState, int duration)
		{
			this.FromState = fromState;
			this.ToState = toState;
			this.Duration = duration;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is BufferedPaintTransition<TState>))
			{
				return this.Equals(obj);
			}
			return ((IEquatable<BufferedPaintTransition<TState>>)this).Equals((BufferedPaintTransition<TState>)obj);
		}

		public override int GetHashCode()
		{
			return (this.FromState ?? 0).GetHashCode() ^ (this.ToState ?? 0).GetHashCode();
		}

		bool System.IEquatable<AddinExpress.Installer.WiXDesigner.BufferedPaintTransition<TState>>.Equals(BufferedPaintTransition<TState> other)
		{
			if (!object.Equals(this.FromState, other.FromState))
			{
				return false;
			}
			return object.Equals(this.ToState, other.ToState);
		}
	}
}