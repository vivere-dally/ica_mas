using core;

namespace masSharp.Message
{
	public record CaptureRequest(IAgent Agent, int TargetX, int TargetY);
	public record CaptureResponse(bool IsSuccessful);
}
