using masSharp.Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace masSharp.Pursuit
{
	public class PursuerAgent : PositionalAgent
	{
		public PursuerAgent(string name, Game game) : base(name, game, AgentType.PURSUER)
		{
		}

		protected override (int, int) GetUpcomingDirection(AgentType[] types, int[] xs, int[] ys)
		{
			var pursuerDirections = new List<(int, int)>();
			var evaderDirections = new List<(int, int)>();
			foreach (var (type, x, y) in types.Zip(xs, ys))
			{
				if (IsAdjucent(base.x, base.y, x, y) && type == AgentType.EVADER)
				{
					var captureResponse = game
						.Ask<CaptureRequest, CaptureResponse>(new(this, x, y))
						.Result;

					if (captureResponse.IsSuccessful)
					{
						L($"Captured from ({base.x}, {base.y}) to ({x}, {y})");
						continue;
					}
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

			//if (evaderDirections.Count == 0 && pursuerDirections.Count == 0)
			//{
			//	var rnd = new Random();
			//	var dX = rnd.Next(-1, 2);
			//	var dY = rnd.Next(-1, 2);
			//	return (dX, dY);
			//}

			if (evaderDirections.Count == 0)
			{
				var rnd = new Random();
				var dX = rnd.Next(-1, 2);
				var dY = rnd.Next(-1, 2);
				return (dX, dY);
			}

			//if (evaderDirections.Count == 0)
			//{
			//	return ComputeDirection(pursuerDirections, true);
			//}

			return ComputeDirection(evaderDirections, true);
		}
	}
}
