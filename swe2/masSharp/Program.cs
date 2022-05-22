using core;
using masSharp.Pursuit;
using System;
using System.Threading;

namespace masSharp
{
	class Program
	{
		static void Main(string[] args)
		{
			var game = new Game();
			while (game.IsRunning)
			{
			}

			Console.WriteLine("closing");
			//A a = new();
			//a.Tell<string>("foo");
			//Console.ReadKey();
		}
	}
}
