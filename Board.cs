﻿using System;
using System.Linq;
using System.Text;

namespace mines
{
	internal class Board
	{
		private int width;
		private int height;

		bool[] mines;
		bool[] flags;
		int[] seen;
		public bool hitMine = false;

		public Board(int width, int height, int nMines)
		{
			this.width = width;
			this.height = height;

			mines = new bool[width * height];
			flags = new bool[width * height];
			seen = new int[width * height]; // -1 if unseen, 0+ if seen (pre calc mine neighbors)
			for (int i = 0; i < seen.Length; i++)
				seen[i] = -1;

			SpreadMines(nMines);
		}

		public int CountFlags()
		{
			return flags.Count(x => x);
		}

		private void SpreadMines(int nMines)
		{
			if (nMines >= width * height)
				throw new ArgumentException("too many mines for board size: " + nMines);

			var rand = new Random();

			while (nMines > 0)
			{
				int pos = rand.Next(width * height);
				if (!mines[pos])
				{
					mines[pos] = true;
					nMines--;
				}
			}
		}

		internal void ShowAll()
		{
			for (int row = 0; row < height; row++)
			{
				for (int col = 0; col < width; col++)
				{
					PeekAt(col, row);
				}
			}
		}

		private int PosIndexFor(int col, int row)
		{
			return col + row * width;
		}

		public bool HasMineAt(int col, int row)
		{
			if (col < 0 || col >= width || row < 0 || row >= height) return false;
			return mines[PosIndexFor(col, row)];
		}

		public bool HasFlagAt(int col, int row)
		{
			if (col < 0 || col >= width || row < 0 || row >= height) return false;
			return flags[PosIndexFor(col, row)];
		}

		public void SetFlagAt(int col, int row, bool flag)
		{
			flags[PosIndexFor(col, row)] = flag;
		}

		public int SeenAt(int col, int row)
		{
			return seen[PosIndexFor(col, row)];
		}

		public bool AllSeen()
		{
			if (CountFlags() != CountMines())
				return false;
			
			for (int row = 0; row < height; row++)
			{
				for (int col = 0; col < width; col++)
				{
					if (SeenAt(col, row) < 0 && !HasFlagAt(col, row)) return false;
				}
			}

			return true;
		}

		private int CountMines()
		{
			return mines.Count(x => x);
		}

		public void PeekAt(int col, int row)
		{
			if (col < 0 || col >= width || row < 0 || row >= height) return;

			if (SeenAt(col, row) >= 0)
			{
				return;
			}

			int signal = CalcSignalFor(col, row);
			seen[PosIndexFor(col, row)] = signal;

			if (HasMineAt(col, row))
			{
				hitMine = true;
			}
			else if (signal == 0)
			{
				ExpandPeekFrom(col, row);
			}
		}

		public void ExpandPeekFrom(int col, int row)
		{
			//TODO account for positive numbers and flags around (middle-click)
			int flagsAround = 0;
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i == j && i == 0) continue;
					if (HasFlagAt(col + j, row + i)) flagsAround++;
				}
			}

			if (flagsAround != SeenAt(col, row)) return;

			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i == j && i == 0) continue;
					if (HasFlagAt(col + j, row + i)) continue;

					PeekAt(col + j, row + i);

				}
			}
		}

		private int CalcSignalFor(int col, int row)
		{
			int sum = 0;

			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i == j && i == 0) continue;

					if (HasMineAt(col + j, row + i))
						sum++;
				}
			}

			return sum;
		}

		public string GetGrid()
		{
			StringBuilder res = new StringBuilder();

			int padLeftCount = (int)Math.Ceiling(Math.Log10(height + 1));
			string padLeft = "".PadLeft(padLeftCount);

			res.Append(padLeft + " ");
			for (int i = 0; i < width; i++)
			{
				res.Append((char)('A' + i));
			}
			res.AppendLine();

			string topWall = padLeft + "+".PadRight(width + 1, '-') + "+";
			res.AppendLine(topWall);
			for (int row = 0; row < height; row++)
			{
				res.Append(String.Format("{0," + padLeftCount + "}|", row + 1));
				for (int col = 0; col < width; col++)
				{
					res.Append(GetCharacterForPos(row, col));
				}
				res.AppendLine("|");

			}
			res.AppendLine(topWall);
			return res.ToString();
		}

		private string GetCharacterForPos(int row, int col)
		{
			if (HasFlagAt(col, row))
			{
				return "F";
			}
			else
			{
				int nearby = SeenAt(col, row);
				if (nearby == -1)
				{
					if (hitMine && HasMineAt(col, row)) return "x";
					else return ".";
				}
				else
				{
					if (HasMineAt(col, row)) return "X";
					else if (nearby == 0) return " ";
					else return nearby.ToString();
				}
			}
		}
	}
}