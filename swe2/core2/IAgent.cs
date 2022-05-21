using System.Threading.Tasks;

namespace core
{
	public interface IAgent
	{
		/// <summary>
		/// Fire and forget message
		/// </summary>
		/// <typeparam name="T">Message type</typeparam>
		/// <param name="message">The message</param>
		void Tell<T>(T message);

		/// <summary>
		/// Request and response message
		/// </summary>
		/// <typeparam name="TIn">Message type</typeparam>
		/// <typeparam name="TOut">Return type</typeparam>
		/// <param name="message">The message</param>
		/// <returns></returns>
		Task<TOut> Ask<TIn, TOut>(TIn message);

		void Tell<TIn, TOut>(IAgent sender, TIn message);

		string Name { get; }
	}
}

