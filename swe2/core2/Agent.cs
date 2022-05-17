using core.Exception;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace core
{
	public abstract class Agent : IAgent
	{
		private AgentSystem agentSystem;
		private readonly IDictionary<Type, Func<dynamic, dynamic>> handlers;
		private readonly ISet<Type> handlerReturnTypes;
		private readonly IDictionary<Type, Action<dynamic>> handlerResponses;

		public Agent()
		{
			this.handlers = new Dictionary<Type, Func<object, object>>();
			this.handlerReturnTypes = new HashSet<Type>();
			this.handlerResponses = new Dictionary<Type, Action<dynamic>>();
		}

		Task IAgent.Start(AgentSystem agentSystem)
		{
			this.agentSystem = agentSystem;
			return Task.Run(() =>
			{
				this.agentSystem.agents.TryGetValue(this, out var q);
				while (true)
				{
					if (q.Count == 0)
					{
						Task.Delay(TimeSpan.FromMilliseconds(500)).Wait();
						continue;
					}

					var obj = q.Dequeue();
					var handler = this.handlers[obj.GetType()];
					var result = handler(obj);
					if (result == null)
					{
						continue;
					}

					if (this.handlerResponses.TryGetValue(result.GetType(), out Action<dynamic> action))
					{
						action(result);
					}
				}
			});
		}

		public void Tell<T>(T message)
		{
			this.ThrowIfHandlerDoesNotExists<T>();
			if (this.agentSystem.agents.TryGetValue(this, out var q))
			{
				q.Enqueue(message);
			}
		}

		public Task<TOut> Ask<TIn, TOut>(TIn message)
		{
			this.ThrowIfHandlerDoesNotExists<TIn>();
			this.ThrowIfHandlerReturnTypeDoesNotExists<TOut>();

			var tcs = new TaskCompletionSource<TOut>();
			this.handlerResponses.Add(typeof(TOut), (any) =>
			{
				tcs.TrySetResult(any);
			});

			return tcs.Task;
		}

		protected void Handle<T>(Action<T> handler)
		{
			this.ThrowIfHandlerExists<T>();
			this.handlers.Add(typeof(T), (any) =>
			{
				handler(any);
				return null;
			});
		}

		protected void Handle<TIn, TOut>(Func<TIn, TOut> handler)
		{
			this.ThrowIfHandlerExists<TIn>();
			this.handlers.Add(typeof(TIn), (any) => handler(any));
			this.handlerReturnTypes.Add(typeof(TOut));
		}

		private void ThrowIfHandlerExists<T>()
		{
			if (this.handlers.ContainsKey(typeof(T)))
			{
				throw new DuplicateAgentActionException<T>();
			}
		}

		private void ThrowIfHandlerDoesNotExists<T>()
		{
			if (!this.handlers.ContainsKey(typeof(T)))
			{
				throw new AgentActionNotFoundException<T>();
			}
		}

		private void ThrowIfHandlerReturnTypeDoesNotExists<T>()
		{
			if (!this.handlers.ContainsKey(typeof(T)))
			{
				throw new AgentActionReturnTypeNotFoundException<T>();
			}
		}
	}
}
