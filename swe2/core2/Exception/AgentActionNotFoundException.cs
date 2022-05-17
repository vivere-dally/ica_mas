using System;

namespace core.Exception
{
	[Serializable]
	public class AgentActionNotFoundException<T> : System.Exception
	{
		public AgentActionNotFoundException() : base($"There is no handler registered for type {typeof(T)}.")
		{
		}
	}
}
