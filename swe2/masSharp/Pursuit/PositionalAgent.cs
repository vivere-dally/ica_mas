using core;
using masSharp.Message;
using System;
using System.Collections.Generic;

namespace masSharp.Pursuit
{
	public abstract class PositionalAgent : Agent
	{
		private readonly string name;
		protected readonly Game game;
		private readonly AgentType agentType;
		protected int x;
		protected int y;

		public PositionalAgent(string name, Game game, AgentType agentType) : base(name)
		{
			this.name = name;
			this.game = game;
			this.agentType = agentType;

			Handle<AgentTypeRequest, AgentTypeResponse>((_) =>
			{
				return new(this.agentType);
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

			Handle<MoveCommandRequest, MoveCommandResponse>((_) =>
			{
				var (types, xs, ys) = game
					.Ask<SurroundingObservationRequest, SurroundingObservationResponse>(
						new(this.agentType, this.x, this.y))
					.Result;
				var (dX, dY) = this.GetUpcomingDirection(types, xs, ys);

				int newX = this.x + dX;
				if (newX < 0 || newX >= Config.MAP_LENGTH)
				{
					dX = 0;
				}

				int newY = this.y + dY;
				if (newY < 0 || newY >= Config.MAP_LENGTH)
				{
					dY = 0;
				}

				if ((dX, dY) == (0, 0))
				{
					L("will stay put");
					return new();
				}

				newX = this.x + dX;
				newY = this.y + dY;
				var response = game
					.Ask<MoveRequest, MoveResponse>(
						new(this, this.x, this.y, newX, newY))
					.Result;

				if (response.IsSuccessful)
				{
					L($"moved from ({x}, {y}) to ({newX}, {newY})");
					x = newX;
					y = newY;
				}
				else
				{
					L($"Could not move to ({newX}, {newY})");
				}

				return new();
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

		protected abstract (int, int) GetUpcomingDirection(AgentType[] types, int[] xs, int[] ys);

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

		protected static (int, int) ComputeDirection(List<(int, int)> directions, bool chase)
		{
			int left = 0, right = 0, up = 0, down = 0;
			foreach (var (x, y) in directions)
			{
				left += x < 0 ? 1 : 0;
				right += x > 0 ? 1 : 0;
				up += y < 0 ? 1 : 0;
				down += y > 0 ? 1 : 0;
			}

			int modifier = chase ? 1 : -1;
			int dX = left <= right ? -1 * modifier : 1 * modifier;
			int dY = up <= right ? -1 * modifier: 1 * modifier;

			return (dX, dY);
		}
	}
}
