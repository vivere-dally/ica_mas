using core;

namespace masSharp
{
	public class A : Agent
	{
		private readonly B b;

		public A() : base("A")
		{
			b = new(this);
			Handle<int, int>((num) =>
			{
				return num + 1;
			});

			Handle<string>((n) =>
			{
				var v = b.Ask<int, int>(0).Result;
				System.Console.WriteLine(n + v);
			});
		}
	}
}
