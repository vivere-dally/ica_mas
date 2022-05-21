using masSharp.Message;
using System.Collections.Generic;
using System.Linq;

namespace masSharp.Pursuit
{
	public class EvaderAgent : PositionalAgent
	{
		public EvaderAgent(string name, Game game) : base(name, game, AgentType.EVADER)
		{
		}

		protected override (int, int) GetUpcomingDirection(AgentType[] types, int[] xs, int[] ys)
		{
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
				return (0, 0);
			}

			return ComputeDirection(pursuerDirections, false);
		}
	}
}
