using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core
{
	public class AgentSystem
	{
		private readonly string name;

		private AgentSystem(string name)
		{
			this.name = name;
		}

		public static AgentSystem Create(string name)
		{
			return new AgentSystem(name);
		}
	}
}
