using core;
using masSharp.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace masSharp.Pursuit.Component
{
	public class PositionManager : Agent
	{
		private readonly ConcurrentDictionary<IAgent, Tuple<int, int>> agentToPosition = new();
		private readonly ConcurrentDictionary<Tuple<int, int>, IAgent> positionToAgent = new();

		public PositionManager() : base("pm")
		{
			Handle<LockPositionRequest, LockPositionResponse>((request) =>
			{
				var (agent, x, y) = request;
				var response = new LockPositionResponse(!positionToAgent.ContainsKey(new(x, y)));
				if (response.IsLocked)
				{
					positionToAgent[new(x, y)] = agent;
					agentToPosition[agent] = new(x, y);
				}

				return response;
			});
			Handle<ObservationRequest, ObservationResponse>((request) => CircularObservation(request.AgentType, request.X, request.Y));
		}

		private ObservationResponse CircularObservation(AgentType agentType, int x, int y)
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
