using System;

namespace core.Exception
{
	[Serializable]
	public class AgentActionNotSupportedException<T> : System.Exception
	{
		public AgentActionNotSupportedException() : base($"There is no handler registered for type {nameof(T)}.")
		{
		}
	}
}
