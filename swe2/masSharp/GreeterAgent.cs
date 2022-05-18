using System;
using core;

namespace masSharp
{
	public class GreeterAgent : Agent2
	{
		public GreeterAgent()
		{
			Handle<string>((name) =>
			{
				Console.WriteLine($"Hello {name}");
			});

			Handle<int, int>((num) => {
				return num + 1;
			});
		}
	}
}
