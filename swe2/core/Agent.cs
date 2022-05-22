using ConcurrentCollections;
using core.DTO;
using core.Exception;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace core
{
	public abstract class Agent : IAgent
	{
		private readonly CancellationTokenSource cts = new();
		private ulong isRunning = 0L;

		private readonly ConcurrentDictionary<Type, Func<dynamic, dynamic>> handlers = new();
		private readonly ConcurrentHashSet<Tuple<Type, Type>> handlerReturnTypes = new();

		private readonly ConcurrentDictionary<Guid, Action<dynamic>> defferedResponses = new();

		private readonly ConcurrentQueue<QueueItem> q = new();
		private readonly string name;

		protected Agent(string name)
		{
			this.name = name;

			Console.CancelKeyPress += Console_CancelKeyPress;

			this.Start();
		}

		private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			L("shutting down...");
			Interlocked.Increment(ref isRunning);
			cts.Cancel();
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
			}, cts.Token);
		}

		public string Name { get => this.name; }
		public bool IsRunning { get => Interlocked.Read(ref isRunning) == 0; }
		protected CancellationToken CancellationToken { get => cts.Token; }

		public void Tell<T>(T message)
		{
			this.ThrowIfHandlerDoesNotExists<T>();
			this.q.Enqueue(new QueueItem(Guid.NewGuid(), message));
		}

		public void Tell<TIn, TOut>(IAgent sender, TIn message)
		{
			this.ThrowIfHandlerDoesNotExists<TIn>();

			var correlationId = Guid.NewGuid();
			this.defferedResponses.TryAdd(correlationId, (any) =>
			{
				if (any != null)
				{
					sender.Tell<TOut>(any);
				}
			});

			this.q.Enqueue(new QueueItem(correlationId, message));
		}

		public Task<TOut> Ask<TIn, TOut>(TIn message)
		{
			this.ThrowIfHandlerDoesNotExists<TIn>();
			this.ThrowIfHandlerReturnTypeDoesNotExists<TIn, TOut>();

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
			this.handlerReturnTypes.Add(new Tuple<Type, Type>(typeof(TIn), typeof(TOut)));
		}

		public void L(dynamic message)
		{
			Console.WriteLine($"[{this.name}]@[{DateTime.UtcNow.TimeOfDay}]-{message}");
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

		private void ThrowIfHandlerReturnTypeDoesNotExists<TIn, TOut>()
		{
			if (!this.handlerReturnTypes.Contains(new Tuple<Type, Type>(typeof(TIn), typeof(TOut))))
			{
				throw new AgentActionReturnTypeNotFoundException<TIn, TOut>();
			}
		}
	}
}

