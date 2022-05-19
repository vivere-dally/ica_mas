using core;
using masSharp.Message.Position;
using System;
using System.Collections.Generic;

namespace masSharp.Pursuit.Component
{
	public class PositionManager : Agent
	{
		private readonly Dictionary<IAgent, Tuple<int, int>> agentToPosition = new();
		private readonly HashSet<Tuple<int, int>> positions = new();

		public PositionManager()
		{
			Handle<IsPositionOccupied, bool>((pos) =>
			{
				if (positions.Contains(pos))
				{
					return false;
				}

				return true;
			});
		}
	}
}
