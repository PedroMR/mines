using System;
using System.IO;
using NLog;

namespace mines
{
	class MainClass
	{
		public static void Main(string[] args)
		{

			var input = Console.OpenStandardInput();
			var reader = new StreamReader(input);

			while (true)
			{
				Console.WriteLine("Console Minesweeper");
				Console.WriteLine("===================\n");
				Game game = new Game();
				while (game.IsRunning())
				{
					Console.WriteLine(game.GetState());
					Console.WriteLine("A3 to peek at grid position A3");
					Console.WriteLine("f A3 to flag grid position A3");

					var line = reader.ReadLine();
					game.Parse(line);

				}

				Console.WriteLine(game.GetState());
				Console.WriteLine("\nGAME OVER\n");
			}
		}
	}
}
