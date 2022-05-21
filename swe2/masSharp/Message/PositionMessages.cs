using core;
using masSharp.Pursuit;

namespace masSharp.Message
{
	public record LockPositionRequest(IAgent Agent, int X, int Y);
	public record LockPositionResponse(bool IsLocked);

	public record MoveCommandRequest();
	public record MoveCommandResponse();

	public record MoveRequest(IAgent Agent, int X, int Y, int NewX, int NewY);
	public record MoveResponse(bool IsSuccessful);

	public record SurroundingObservationRequest(AgentType AgentType, int X, int Y);
	public record SurroundingObservationResponse(AgentType[] AgentTypes, int[] Xs, int[] Ys);
}
