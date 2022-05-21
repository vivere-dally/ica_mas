using core;
using masSharp.Message;
using System;

namespace masSharp.Pursuit
{
	public class PositionalAgent : Agent
	{
		protected readonly Game game;
		protected int x;
		protected int y;

		public PositionalAgent(string name, Game game) : base(name)
		{
			this.game = game;
			Handle<LockPositionResponse>((response) =>
			{
				if (!response.IsLocked)
				{
					GeneratePosition();
					this.game.Tell<LockPositionRequest, LockPositionResponse>(this, new(this, x, y));
					return;
				}

				L($"Position: ({x}, {y})");
			});

			GeneratePosition();
			this.game.Tell<LockPositionRequest, LockPositionResponse>(this, new(this, x, y));
		}

		private void GeneratePosition()
		{
			var rnd = new Random();
			x = rnd.Next(0, Config.MAP_LENGTH);
			y = rnd.Next(0, Config.MAP_LENGTH);
		}
	}
}
