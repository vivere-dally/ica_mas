using System;
using System.Collections.Generic;

namespace core
{
	public class AgentSystem
	{
		private readonly string name;
		internal readonly IDictionary<IAgent, Queue<dynamic>> agents;

		public AgentSystem(string name)
		{
			this.name = name;
			this.agents = new Dictionary<IAgent, Queue<dynamic>>();
		}

		public IAgent AgentOf<T>(params object[] args) where T : IAgent
		{
			IAgent agent = (IAgent)Activator.CreateInstance(typeof(T), args);
			this.agents.Add(agent, new Queue<dynamic>());
			agent.Start(this);

			return agent;
		}
	}
}
