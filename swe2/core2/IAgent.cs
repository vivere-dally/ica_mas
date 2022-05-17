using System.Threading.Tasks;

namespace core
{
	public interface IAgent
	{
		void Tell<T>(T message);
		Task<TOut> Ask<TIn, TOut>(TIn message);
		internal Task Start(AgentSystem agentSystem);
	}
}
