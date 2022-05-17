using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace core
{
	public class Props
	{

		private Props()
		{
		}

		public static Props Create<TAgent>(Expression<Func<TAgent>> factory) where TAgent : IAgent
		{
			return new DynamicProps<TAgent>(factory.Compile());
		}

		internal class DynamicProps<TAgent> : Props where TAgent : IAgent
		{
			private readonly Func<TAgent> factory;

			internal DynamicProps(Func<TAgent> factory)
			{
				this.factory = factory;
			}

			public TAgent NewAgent()
			{
				return this.factory.Invoke();
			}
		}
	}
}
