using core;
using masSharp.Message;
using System;
using System.Threading.Tasks;

namespace masSharp.Pursuit
{
	public abstract class PositionalAgent : Agent
	{
		protected readonly Game game;
		protected int x;
		protected int y;

		public PositionalAgent(string name, Game game) : base(name)
		{
			this.game = game;

			Handle<MoveRequest, MoveResponse>((_) =>
			{
				this.Move().Wait();
				return new();
			});

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

		protected abstract Task Move();

		protected static bool IsAdjucent(int x1, int y1, int x2, int y2)
		{
			if (x1 == x2)
			{
				return y1 == y2 - 1 || y1 == y2 + 1;
			}

			if (y1 == y2)
			{
				return x1 == x2 - 1 || x1 == x2 + 1;
			}

			return (x1 == x2 - 1 && y1 == y2 - 1) ||
				(x1 == x2 - 1 && y1 == y2 + 1) ||
				(x1 == x2 + 1 && y1 == y2 - 1) ||
				(x2 == x2 + 1 && y2 == y2 + 1);
		}

		protected static (int, int) GetDirection(int x1, int y1, int x2, int y2)
		{
			int x = x1 == x2 ? 0 : x1 > x2 ? -1 : 1;
			int y = y1 == y2 ? 0 : y1 > y2 ? -1 : 1;

			return (x, y);
		}
	}
}
