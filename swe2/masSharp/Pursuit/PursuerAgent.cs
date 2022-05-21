using masSharp.Message;

namespace masSharp.Pursuit
{
	public class PursuerAgent : PositionalAgent
	{
		public PursuerAgent(string name, Game environment) : base(name, environment)
		{
			Handle<AgentTypeRequest, AgentTypeResponse>((_) =>
			{
				return new(AgentType.PURSUER);
			});

		}
	}
}
