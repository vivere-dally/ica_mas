using masSharp.Message;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masSharp.Pursuit
{
	public class PursuerAgent : PositionalAgent
	{
		public PursuerAgent(string name, Game game) : base(name, game)
		{
			Handle<AgentTypeRequest, AgentTypeResponse>((_) =>
			{
				return new(AgentType.PURSUER);
			});
		}

		protected override async Task Move()
		{
			var (types, xs, ys) = await game.Ask<SurroundingObservationRequest, SurroundingObservationResponse>(new(AgentType.PURSUER, base.x, base.y));

			var pursuerDirections = new List<(int, int)>();
			var evaderDirections = new List<(int, int)>();
			foreach (var (type, x, y) in types.Zip(xs, ys))
			{
				if (IsAdjucent(base.x, base.y, x, y) && type == AgentType.EVADER)
				{
					// TODO capture
					continue;
				}

				var direction = GetDirection(base.x, base.y, x, y);
				if (type == AgentType.EVADER)
				{
					evaderDirections.Add(direction);
				}
				else
				{
					pursuerDirections.Add(direction);
				}
			}

			if (evaderDirections.Count == 0)
			{
				// TODO move towards other pursuers
				return;
			}

			// TODO move towards evaders
		}
	}
}
