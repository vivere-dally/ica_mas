using System;

namespace core.Exception
{
	[Serializable]
	class AgentActionReturnTypeNotFoundException<T> : System.Exception
	{
		public AgentActionReturnTypeNotFoundException() : base($"There is no handler registered with return type type {typeof(T)}.")
		{
		}
	}
}
