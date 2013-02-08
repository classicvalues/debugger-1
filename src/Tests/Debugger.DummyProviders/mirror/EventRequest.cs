using System;
using Debugger.Backend;

namespace Debugger.DummyProviders
{
	class EventRequest : BaseMirror, IEventRequest
	{
		public event Action<IEventRequest> RequestEnabled;
		public event Action<IEventRequest> RequestDisabled;

		public virtual void Enable ()
		{
			if (RequestEnabled != null)
				RequestEnabled (this);
		}

		public virtual void Disable ()
		{
			if (RequestDisabled != null)
				RequestDisabled (this);
		}
	}
}