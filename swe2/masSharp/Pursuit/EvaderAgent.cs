using core;
using masSharp.Message;

namespace masSharp.Pursuit
{
	public class EvaderAgent : PositionalAgent
	{
		public EvaderAgent(string name, Game environment) : base(name, environment)
		{
			Handle<AgentTypeRequest, AgentTypeResponse>((_) =>
			{
				return new(AgentType.EVADER);
			});
		}
	}
}
