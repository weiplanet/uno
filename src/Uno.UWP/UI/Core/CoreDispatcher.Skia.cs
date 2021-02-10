﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Windows.UI.Core
{
	public sealed partial class CoreDispatcher
	{
		/// <summary>
		/// Provide a action that will delegate the dispach of CoreDispatcher work
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Action<Action> DispatchOverride;

		/// <summary>
		/// Provide a action that will delegate the dispach of CoreDispatcher work
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Func<bool> HasThreadAccessOverride;

		private bool GetHasThreadAccess()
		{
			if (HasThreadAccessOverride == null)
			{
				throw new InvalidOperationException("CoreDispatcher.HasThreadAccessOverride must be set");
			}

			return HasThreadAccessOverride();
		}

		public static CoreDispatcher Main { get; } = new CoreDispatcher();


		partial void EnqueueNative()
		{
			if(DispatchOverride == null)
			{
				throw new InvalidOperationException("CoreDispatcher.DispatchOverride must be set");
			}

			DispatchOverride(() => DispatchItems());
		}
	}
}
