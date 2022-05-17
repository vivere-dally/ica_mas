using System;

namespace core.Exception
{
	[Serializable]
	public class DuplicateAgentActionException<T> : System.Exception
	{
		public DuplicateAgentActionException() : base($"There is already an handler for type {nameof(T)}.")
		{
		}
	}
}
