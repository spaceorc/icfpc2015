﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeSecretProject.Logic
{
	public class Cell
	{
		public bool Locked;
		public int X;
		public int Y;

		public Cell Move(Vector vector)
		{
			return new Cell {X = X + vector.X, Y = Y + vector.Y, Locked = Locked};
		}

		public Cell RotateCW(Cell pivot)
		{
			throw new InvalidOperationException("TODO");
		}

		public Cell RotateCCW(Cell pivot)
		{
			throw new InvalidOperationException("TODO");
		}
	}
}
