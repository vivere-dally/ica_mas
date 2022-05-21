using core;
using masSharp.Pursuit;
using System;

namespace masSharp.Message
{
	public record LockPositionRequest(IAgent agent, int X, int Y);
	public record LockPositionResponse(bool IsLocked);

	public record ObservationRequest(AgentType AgentType, int X, int Y);
	public record ObservationResponse(AgentType[] AgentTypes, int[] Xs, int[] Ys);
}
