using core;
using masSharp.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace masSharp.Pursuit
{
	public class Game : Agent
	{
		private readonly IAgent[] agents = new IAgent[Config.NO_AGENTS];

		private readonly ConcurrentDictionary<IAgent, Tuple<int, int>> agentToPosition = new();
		private readonly ConcurrentDictionary<Tuple<int, int>, IAgent> positionToAgent = new();

		private ulong no_initialized_agents = 0;
		private ulong no_evaders_left = Config.NO_EVADERS;

		public Game() : base("game")
		{
			Handle<LockPositionRequest, LockPositionResponse>((request) =>
			{
				var (agent, x, y) = request;
				var response = new LockPositionResponse(!positionToAgent.ContainsKey(new(x, y)));
				if (response.IsLocked)
				{
					positionToAgent[new(x, y)] = agent;
					agentToPosition[agent] = new(x, y);
					Interlocked.Increment(ref no_initialized_agents);
				}

				return response;
			});

			Handle<SurroundingObservationRequest, SurroundingObservationResponse>((request) => CircularObservation(request.AgentType, request.X, request.Y));

			Handle<MoveRequest, MoveResponse>((request) =>
			{
				var (agent, x, y, newX, newY) = request;
				var response = new MoveResponse(!positionToAgent.ContainsKey(new(newX, newY)));
				if (response.IsSuccessful)
				{
					positionToAgent.Remove(new(x, y), out var _);
					positionToAgent[new(newX, newY)] = agent;
					agentToPosition[agent] = new(x, y);
				}

				return response;	
			});

			this.InitializeAgents();
			this.Start();
		}

		private void InitializeAgents()
		{
			for (int i = 0; i < Config.NO_PURSUERS; i++)
			{
				agents[i] = new PursuerAgent($"P{i}", this);
			}

			for (int i = Config.NO_PURSUERS, j = 0; i < Config.NO_AGENTS; i++, j++)
			{
				agents[i] = new EvaderAgent($"E{j}", this);
			}
		}

		private void Start()
		{
			Task.Run(() =>
			{
				while (Interlocked.Read(ref this.no_initialized_agents) < Config.NO_AGENTS)
				{
					Task.Delay(TimeSpan.FromMilliseconds(500)).Wait();
				}

				int roundCount = 1;
				var rnd = new Random();
				while (Interlocked.Read(ref this.no_evaders_left) != 0)
				{
					L($"Round {roundCount}");
					L($"Evaders Left {Interlocked.Read(ref this.no_evaders_left)}");

					var tasks = Enumerable
						.Range(0, Config.NO_AGENTS)
						.OrderBy(_ => rnd.Next())
						.Select((index) => this.agents[index].Ask<MoveCommandRequest, MoveCommandResponse>(new()))
						.ToArray();

					Task.WaitAll(tasks);
					roundCount++;
				}
			});
		}

		private SurroundingObservationResponse CircularObservation(AgentType agentType, int x, int y)
		{
			List<AgentType> agentTypes = new();
			List<int> xs = new();
			List<int> ys = new();

			int observationRange = agentType switch
			{
				AgentType.PURSUER => Config.PURSUER_OBSERVATION_RANGE,
				AgentType.EVADER => Config.EVADER_OBSERVATION_RANGE,
				_ => Config.EVADER_OBSERVATION_RANGE
			};

			int rSq = observationRange * observationRange;
			for (int row = 0; row < Config.MAP_LENGTH; row++)
			{
				int a = row - x;
				if (a < 0)
				{
					continue;
				}

				for (int col = 0; col < Config.MAP_LENGTH; col++)
				{
					int b = col - y;
					if (col < 0)
					{
						continue;
					}

					var position = new Tuple<int, int>(a, b);
					if (a * a + b * b <= rSq && positionToAgent.ContainsKey(position))
					{
						var agent = positionToAgent[position];
						var agentTypeResponse = agent.Ask<AgentTypeRequest, AgentTypeResponse>(new()).Result;
						agentTypes.Add(agentTypeResponse.AgentType);
						xs.Add(a);
						ys.Add(b);
					}
				}
			}

			return new(agentTypes.ToArray(), xs.ToArray(), ys.ToArray());
		}

	}
}
