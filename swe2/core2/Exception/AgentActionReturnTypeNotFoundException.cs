using System;

namespace core.Exception
{
	[Serializable]
	class AgentActionReturnTypeNotFoundException<TIn, TOut> : System.Exception
	{
		public AgentActionReturnTypeNotFoundException()
			: base($"There is no handler registered with return type ({typeof(TIn)}, {typeof(TOut)}).")
		{
		}
	}
}
