using System;

namespace console
{
	class Program
	{
		static void Main(string[] args)
		{
			var engine = new MD.Engine(50,10);
			for(int y = 0; y < engine.Floor.GetLength(1); y++)
			{
				for(int x = 0; x < engine.Floor.GetLength(0); x++)
				{
					Console.Write(engine.Floor[x,y]);
				}
				Console.WriteLine();
			}
		}
	}
}
