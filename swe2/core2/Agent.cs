using ConcurrentCollections;
using core.DTO;
using core.Exception;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace core
{
	public abstract class Agent : IAgent
	{
		private readonly ConcurrentDictionary<Type, Func<dynamic, dynamic>> handlers = new();
		private readonly ConcurrentHashSet<Type> handlerReturnTypes = new();

		private readonly ConcurrentDictionary<Guid, Action<dynamic>> defferedResponses = new();

		private readonly ConcurrentQueue<QueueItem> q = new();

		protected Agent()
		{
			this.Start();
		}

		private void Start()
		{
			Task.Run(() =>
			{
				while (true)
				{
					if (q.IsEmpty)
					{
						Task.Delay(TimeSpan.FromMilliseconds(500)).Wait();
						continue;
					}

					if (!q.TryDequeue(out var queueItem))
					{
						continue;
					}

					var handler = this.handlers[queueItem.Payload.GetType()];
					var result = handler(queueItem.Payload);

					if (this.defferedResponses.TryGetValue(queueItem.CorrelationId, out var action))
					{
						action(result);
						this.defferedResponses.Remove(queueItem.CorrelationId, out var _);
					}
				}
			});
		}

		public void Tell<T>(T message)
		{
			this.ThrowIfHandlerDoesNotExists<T>();
			this.q.Enqueue(new QueueItem(Guid.NewGuid(), message));
		}

		public Task<TOut> Ask<TIn, TOut>(TIn message)
		{
			this.ThrowIfHandlerDoesNotExists<TIn>();
			this.ThrowIfHandlerReturnTypeDoesNotExists<TOut>();

			var correlationId = Guid.NewGuid();
			var tcs = new TaskCompletionSource<TOut>();

			this.defferedResponses.TryAdd(correlationId, (any) =>
			{
				tcs.TrySetResult(any);
			});

			this.q.Enqueue(new QueueItem(correlationId, message));

			return tcs.Task;
		}

		protected void Handle<T>(Action<T> handler)
		{
			this.ThrowIfHandlerExists<T>();
			this.handlers.TryAdd(typeof(T), (any) =>
			{
				handler(any);
				return null;
			});
		}

		protected void Handle<TIn, TOut>(Func<TIn, TOut> handler)
		{
			this.ThrowIfHandlerExists<TIn>();

			this.handlers.TryAdd(typeof(TIn), (any) => handler(any));
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
			if (!this.handlerReturnTypes.Contains(typeof(T)))
			{
				throw new AgentActionReturnTypeNotFoundException<T>();
			}
		}
	}
}

