using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core
{
	public interface IAgent
	{
		public void Tell<T>(T t) where T : Type;
		public void Tell<T>(T t, IAgent sender) where T : Type;
	}
}
