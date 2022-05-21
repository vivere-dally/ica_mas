using core;
using masSharp.Pursuit;

namespace masSharp.Message
{
	public record LockPositionRequest(IAgent Agent, int X, int Y);
	public record LockPositionResponse(bool IsLocked);

	public record MoveRequest();
	public record MoveResponse();

	public record SurroundingObservationRequest(AgentType AgentType, int X, int Y);
	public record SurroundingObservationResponse(AgentType[] AgentTypes, int[] Xs, int[] Ys);
}
