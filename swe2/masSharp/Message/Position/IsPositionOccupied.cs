using System;

namespace masSharp.Message.Position
{
	public class IsPositionOccupied: Tuple<int, int>
	{
		public IsPositionOccupied(Tuple<int, int> position) : base(position.Item1, position.Item2)
		{
		}

		public IsPositionOccupied(int x, int y) : base(x, y)
		{
		}
	}
}

