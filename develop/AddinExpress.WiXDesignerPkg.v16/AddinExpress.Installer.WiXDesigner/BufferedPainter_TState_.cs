using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class BufferedPainter<TState>
	{
		private bool _animationsNeedCleanup;

		private bool _enabled;

		private TState _currentState;

		private TState _newState;

		private TState _defaultState;

		private Size _oldSize;

		public bool BufferedPaintSupported
		{
			get;
			private set;
		}

		public System.Windows.Forms.Control Control
		{
			get;
			private set;
		}

		public int DefaultDuration
		{
			get;
			set;
		}

		public TState DefaultState
		{
			get
			{
				return this._defaultState;
			}
			set
			{
				bool flag = object.Equals(this._currentState, this._defaultState);
				this._defaultState = value;
				if (flag)
				{
					TState tState = this._defaultState;
					TState tState1 = tState;
					this._newState = tState;
					this._currentState = tState1;
				}
			}
		}

		public bool Enabled
		{
			get
			{
				return this._enabled;
			}
			set
			{
				this._enabled = value;
			}
		}

		public TState State
		{
			get
			{
				return this._currentState;
			}
			set
			{
				this._newState = value;
				if (!object.Equals(this._currentState, value))
				{
					if (this._animationsNeedCleanup && this.Control.IsHandleCreated)
					{
						Interop.BufferedPaintStopAllAnimations(this.Control.Handle);
					}
					this.Control.Invalidate();
				}
			}
		}

		public ICollection<BufferedPaintTransition<TState>> Transitions
		{
			get;
			private set;
		}

		public ICollection<VisualStateTrigger<TState>> Triggers
		{
			get;
			private set;
		}

		public BufferedPainter(System.Windows.Forms.Control control)
		{
			this.Transitions = new HashSet<BufferedPaintTransition<TState>>();
			this.Triggers = new HashSet<VisualStateTrigger<TState>>();
			this._enabled = true;
			TState tState = default(TState);
			TState tState1 = tState;
			tState = tState1;
			this._newState = tState1;
			TState tState2 = tState;
			tState = tState2;
			this._currentState = tState2;
			this._defaultState = tState;
			this.Control = control;
			this._oldSize = this.Control.Size;
			this.BufferedPaintSupported = (Environment.OSVersion.Version.Major < 6 || !VisualStyleRenderer.IsSupported ? false : Application.RenderWithVisualStyles);
			this.Control.Resize += new EventHandler(this.Control_Resize);
			this.Control.Disposed += new EventHandler(this.Control_Disposed);
			this.Control.Paint += new PaintEventHandler(this.Control_Paint);
			this.Control.HandleCreated += new EventHandler(this.Control_HandleCreated);
			this.Control.MouseEnter += new EventHandler((object o, EventArgs e) => this.EvalTriggers());
			this.Control.MouseLeave += new EventHandler((object o, EventArgs e) => this.EvalTriggers());
			this.Control.MouseMove += new MouseEventHandler((object o, MouseEventArgs e) => this.EvalTriggers());
			this.Control.GotFocus += new EventHandler((object o, EventArgs e) => this.EvalTriggers());
			this.Control.LostFocus += new EventHandler((object o, EventArgs e) => this.EvalTriggers());
			this.Control.MouseDown += new MouseEventHandler((object o, MouseEventArgs e) => this.EvalTriggers());
			this.Control.MouseUp += new MouseEventHandler((object o, MouseEventArgs e) => this.EvalTriggers());
		}

		public void AddTransition(TState fromState, TState toState, int duration)
		{
			this.Transitions.Add(new BufferedPaintTransition<TState>(fromState, toState, duration));
		}

		public void AddTrigger(VisualStateTriggerTypes type, TState state, Rectangle bounds = null, AnchorStyles anchor = 5)
		{
			this.Triggers.Add(new VisualStateTrigger<TState>(type, state, bounds, anchor));
		}

		private void ApplyCondition(VisualStateTriggerTypes type, ref TState stateIfTrue)
		{
			Func<VisualStateTrigger<TState>, bool> func = null;
			ICollection<VisualStateTrigger<TState>> triggers = this.Triggers;
			Func<VisualStateTrigger<TState>, bool> func1 = func;
			if (func1 == null)
			{
				Func<VisualStateTrigger<TState>, bool> func2 = (VisualStateTrigger<TState> x) => x.Type == type;
				Func<VisualStateTrigger<TState>, bool> func3 = func2;
				func = func2;
				func1 = func3;
			}
			foreach (VisualStateTrigger<TState> visualStateTrigger in triggers.Where<VisualStateTrigger<TState>>(func1))
			{
				if (visualStateTrigger == null)
				{
					continue;
				}
				Rectangle rectangle = (visualStateTrigger.Bounds != Rectangle.Empty ? visualStateTrigger.Bounds : this.Control.ClientRectangle);
				bool flag = rectangle.Contains(this.Control.PointToClient(Cursor.Position));
				bool focused = true;
				VisualStateTriggerTypes visualStateTriggerType = type;
				if (visualStateTriggerType == VisualStateTriggerTypes.Focused)
				{
					focused = this.Control.Focused;
					flag = true;
				}
				else if (visualStateTriggerType == VisualStateTriggerTypes.Pushed)
				{
					focused = (System.Windows.Forms.Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left;
				}
				if (!(focused & flag))
				{
					continue;
				}
				stateIfTrue = visualStateTrigger.State;
			}
		}

		private void CleanupAnimations()
		{
			if (this.Control.InvokeRequired)
			{
				this.Control.Invoke(new MethodInvoker(this.CleanupAnimations));
				return;
			}
			if (this._animationsNeedCleanup)
			{
				if (this.Control.IsHandleCreated)
				{
					Interop.BufferedPaintStopAllAnimations(this.Control.Handle);
				}
				Interop.BufferedPaintUnInit();
				this._animationsNeedCleanup = false;
			}
		}

		private void Control_Disposed(object sender, EventArgs e)
		{
			if (this._animationsNeedCleanup)
			{
				Interop.BufferedPaintUnInit();
				this._animationsNeedCleanup = false;
			}
		}

		private void Control_HandleCreated(object sender, EventArgs e)
		{
			if (this.BufferedPaintSupported)
			{
				Interop.BufferedPaintInit();
				this._animationsNeedCleanup = true;
			}
		}

		private void Control_Paint(object sender, PaintEventArgs e)
		{
			IntPtr intPtr;
			IntPtr intPtr1;
			if (!this.BufferedPaintSupported || !this.Enabled)
			{
				this._currentState = this._newState;
				this.OnPaintVisualState(new BufferedPaintEventArgs<TState>(this._currentState, e.Graphics));
			}
			else
			{
				bool flag = !object.Equals(this._currentState, this._newState);
				IntPtr hdc = e.Graphics.GetHdc();
				if (hdc != IntPtr.Zero)
				{
					if (!Interop.BufferedPaintRenderAnimation(this.Control.Handle, hdc))
					{
						BP_ANIMATIONPARAMS bPANIMATIONPARAM = new BP_ANIMATIONPARAMS()
						{
							cbSize = Marshal.SizeOf<BP_ANIMATIONPARAMS>(bPANIMATIONPARAM),
							style = BP_ANIMATIONSTYLE.BPAS_LINEAR,
							dwDuration = 0
						};
						if (flag)
						{
							BufferedPaintTransition<TState> bufferedPaintTransition = this.Transitions.Where<BufferedPaintTransition<TState>>((BufferedPaintTransition<TState> x) => {
								if (!object.Equals(x.FromState, this._currentState))
								{
									return false;
								}
								return object.Equals(x.ToState, this._newState);
							}).SingleOrDefault<BufferedPaintTransition<TState>>();
							bPANIMATIONPARAM.dwDuration = (bufferedPaintTransition != null ? bufferedPaintTransition.Duration : this.DefaultDuration);
						}
						Rectangle clientRectangle = this.Control.ClientRectangle;
						IntPtr intPtr2 = Interop.BeginBufferedAnimation(this.Control.Handle, hdc, ref clientRectangle, BP_BUFFERFORMAT.BPBF_COMPATIBLEBITMAP, IntPtr.Zero, ref bPANIMATIONPARAM, out intPtr, out intPtr1);
						if (intPtr2 == IntPtr.Zero)
						{
							this.OnPaintVisualState(new BufferedPaintEventArgs<TState>(this._currentState, e.Graphics));
						}
						else
						{
							if (intPtr != IntPtr.Zero)
							{
								this.OnPaintVisualState(new BufferedPaintEventArgs<TState>(this._currentState, Graphics.FromHdc(intPtr)));
							}
							if (intPtr1 != IntPtr.Zero)
							{
								this.OnPaintVisualState(new BufferedPaintEventArgs<TState>(this._newState, Graphics.FromHdc(intPtr1)));
							}
							this._currentState = this._newState;
							Interop.EndBufferedAnimation(intPtr2, true);
						}
					}
					e.Graphics.ReleaseHdc(hdc);
					return;
				}
			}
		}

		private void Control_Resize(object sender, EventArgs e)
		{
			if (this._animationsNeedCleanup && this.Control.IsHandleCreated)
			{
				Interop.BufferedPaintStopAllAnimations(this.Control.Handle);
			}
			foreach (VisualStateTrigger<TState> trigger in this.Triggers)
			{
				if (trigger.Bounds == Rectangle.Empty)
				{
					continue;
				}
				Rectangle bounds = trigger.Bounds;
				if ((trigger.Anchor & AnchorStyles.Left) != AnchorStyles.Left)
				{
					bounds.X = bounds.X + (this.Control.Width - this._oldSize.Width);
				}
				if ((trigger.Anchor & AnchorStyles.Top) != AnchorStyles.Top)
				{
					bounds.Y = bounds.Y + (this.Control.Height - this._oldSize.Height);
				}
				if ((trigger.Anchor & AnchorStyles.Right) == AnchorStyles.Right)
				{
					bounds.Width = bounds.Width + (this.Control.Width - this._oldSize.Width);
				}
				if ((trigger.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
				{
					bounds.Height = bounds.Height + (this.Control.Height - this._oldSize.Height);
				}
				trigger.Bounds = bounds;
			}
			this._oldSize = this.Control.Size;
		}

		private void EvalTriggers()
		{
			if (!this.Triggers.Any<VisualStateTrigger<TState>>())
			{
				return;
			}
			TState defaultState = this.DefaultState;
			this.ApplyCondition(VisualStateTriggerTypes.Focused, ref defaultState);
			this.ApplyCondition(VisualStateTriggerTypes.Hot, ref defaultState);
			this.ApplyCondition(VisualStateTriggerTypes.Pushed, ref defaultState);
			this.State = defaultState;
		}

		protected virtual void OnPaintVisualState(BufferedPaintEventArgs<TState> e)
		{
			if (this.PaintVisualState != null)
			{
				this.PaintVisualState(this, e);
			}
		}

		public event EventHandler<BufferedPaintEventArgs<TState>> PaintVisualState;
	}
}