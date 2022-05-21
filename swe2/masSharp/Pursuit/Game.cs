using core;
using masSharp.Message;
using masSharp.Pursuit.Component;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace masSharp.Pursuit
{
	public class Game : Agent
	{
		private readonly PositionManager positionManager = new();

		private readonly IAgent[] agents = new IAgent[Config.NO_AGENTS];

		public Game() : base("game")
		{
			Handle<LockPositionRequest, LockPositionResponse>((request) =>
				positionManager.Ask<LockPositionRequest, LockPositionResponse>(request).Result);

			this.InitializeAgents();
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
			for (int _ = 0; _ < Config.NO_ROUNDS; _++)
			{
				var rnd = new Random();
				Parallel.ForEach(
					Enumerable.Range(0, Config.NO_AGENTS).OrderBy(_ => rnd.Next()),
					(index) =>
				{
					// Perform agent actions
				});
			}
		}
	}
}
