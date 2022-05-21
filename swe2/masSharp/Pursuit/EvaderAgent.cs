using masSharp.Message;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masSharp.Pursuit
{
	public class EvaderAgent : PositionalAgent
	{
		public EvaderAgent(string name, Game game) : base(name, game)
		{
			Handle<AgentTypeRequest, AgentTypeResponse>((_) =>
			{
				return new(AgentType.EVADER);
			});
		}

		protected override async Task Move()
		{
			var (types, xs, ys) = await game.Ask<SurroundingObservationRequest, SurroundingObservationResponse>(new(AgentType.EVADER, base.x, base.y));

			var pursuerDirections = new List<(int, int)>();
			foreach (var (type, x, y) in types.Zip(xs, ys))
			{
				if (type == AgentType.PURSUER)
				{
					pursuerDirections.Add(GetDirection(base.x, base.y, x, y));
				}
			}

			if (pursuerDirections.Count == 0)
			{
				return;
			}

			int left = 0, right = 0, up = 0, down = 0;
			foreach (var (x, y) in pursuerDirections)
			{
				left += x < 0 ? 1 : 0;
				right += x > 0 ? 1 : 0;
				up += y < 0 ? 1 : 0;
				down += y > 0 ? 1 : 0;
			}

			int dX = left <= right ? -1 : 1;
			int dY = up <= right ? -1 : 1;

		}
	}
}
