using core;
using masSharp.Message.Position;
using System;

namespace masSharp.Pursuit
{
	public class PositionalAgent : Agent
	{
		protected readonly Game environment;
		protected Tuple<int, int> position;

		public PositionalAgent(Game environment)
		{
			this.environment = environment;
			do
			{
				GeneratePosition();
			} while (!this.environment.Ask<IsPositionOccupied, bool>(new IsPositionOccupied(position)).Result);
			Console.WriteLine($"Position: {position}");
		}

		private void GeneratePosition()
		{
			var rnd = new Random();
			this.position = new Tuple<int, int>(
				rnd.Next(0, Config.MAP_LENGTH),
				rnd.Next(0, Config.MAP_LENGTH));
		}
	}
}
