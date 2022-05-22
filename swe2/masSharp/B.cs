using core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace masSharp
{
	public class B : Agent
	{
		public B(A a) : base("B")
		{
			Handle<int, int>((num) =>
			{
				return a.Ask<int, int>(num).Result + 1;
			});
		}
	}
}
