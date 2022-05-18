using System.Threading.Tasks;

namespace core
{
	public interface IAgent2
	{
		void Tell<T>(T message);
		Task<TOut> Ask<TIn, TOut>(TIn message);
	}
}

