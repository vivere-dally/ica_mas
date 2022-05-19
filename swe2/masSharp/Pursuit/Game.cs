using core;
using masSharp.Message.Position;
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

		public Game()
		{
			// TODO - maybe implement forwarding
			Handle<IsPositionOccupied, bool>((position) => positionManager.Ask<IsPositionOccupied, bool>(position).Result);

			this.InitializeAgents();
		}

		private void InitializeAgents()
		{
			for (int i = 0; i < Config.NO_PURSUERS; i++)
			{
				agents[i] = new PursuerAgent(this);
			}

			for (int i = Config.NO_PURSUERS; i < Config.NO_AGENTS; i++)
			{
				agents[i] = new EvaderAgent(this);
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
