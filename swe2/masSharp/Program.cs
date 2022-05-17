using core;
using System;

namespace masSharp
{
	class Program
	{
		static void Main(string[] args)
		{
			var agentSystem = new AgentSystem("foo");
			var greeter = agentSystem.AgentOf<GreeterAgent>();
			greeter.Tell("Mihai");
			int sum = greeter.Ask<int, int>(123).Result;
			Console.WriteLine(sum);
			greeter.Tell("Ionut");
			Console.ReadKey();
		}
	}
}
1