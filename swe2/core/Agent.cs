using System;
using System.Collections.Generic;
using core.Exception;

namespace core
{
	public abstract class Agent : IAgent
	{
		private readonly IDictionary<Type, Action<Type>> handlers;

		public Agent()
		{
			this.handlers = new Dictionary<Type, Action<Type>>();
		}

		protected void Receive<T>(Action<T> handler) where T : Type
		{
			if (handlers.ContainsKey(typeof(T)))
			{
				throw new DuplicateAgentActionException<T>();
			}

			handlers.Add(typeof(T), obj => handler(obj as T));
		}

		public void Tell<T>(T t) where T : Type
		{
			if (!handlers.ContainsKey(typeof(T)))
			{
				throw new AgentActionNotSupportedException<T>();
			}
		}

		public void Tell<T>(T t, IAgent sender) where T : Type
		{
			if (!handlers.ContainsKey(typeof(T)))
			{
				throw new AgentActionNotSupportedException<T>();
			}
		}

	}
}
