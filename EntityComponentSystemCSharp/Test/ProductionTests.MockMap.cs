using System;
using RogueSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EntityComponentSystemCSharp
{

	public class MockMap : RogueSharp.IMap
	{
		public int Width => 0;

		public int Height => 0;

		public ReadOnlyCollection<ICell> AppendFov(int xOrigin, int yOrigin, int radius, bool lightWalls)
		{
			throw new NotImplementedException();
		}

		public ICell CellFor(int index)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public void Clear(bool isTransparent, bool isWalkable)
		{
			throw new NotImplementedException();
		}

		public IMap Clone()
		{
			throw new NotImplementedException();
		}

		public ReadOnlyCollection<ICell> ComputeFov(int xOrigin, int yOrigin, int radius, bool lightWalls)
		{
			throw new NotImplementedException();
		}

		public void Copy(IMap sourceMap)
		{
			throw new NotImplementedException();
		}

		public void Copy(IMap sourceMap, int left, int top)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ICell> GetAllCells()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ICell> GetBorderCellsInCircle(int xCenter, int yCenter, int radius)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ICell> GetBorderCellsInDiamond(int xCenter, int yCenter, int distance)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ICell> GetBorderCellsInSquare(int xCenter, int yCenter, int distance)
		{
			throw new NotImplementedException();
		}

		public ICell GetCell(int x, int y)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ICell> GetCellsAlongLine(int xOrigin, int yOrigin, int xDestination, int yDestination)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ICell> GetCellsInCircle(int xCenter, int yCenter, int radius)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ICell> GetCellsInColumns(params int[] columnNumbers)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ICell> GetCellsInDiamond(int xCenter, int yCenter, int distance)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ICell> GetCellsInRows(params int[] rowNumbers)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ICell> GetCellsInSquare(int xCenter, int yCenter, int distance)
		{
			throw new NotImplementedException();
		}

		public int IndexFor(int x, int y)
		{
			throw new NotImplementedException();
		}

		public int IndexFor(ICell cell)
		{
			throw new NotImplementedException();
		}

		public void Initialize(int width, int height)
		{
			throw new NotImplementedException();
		}

		public bool IsExplored(int x, int y)
		{
			throw new NotImplementedException();
		}

		public bool IsInFov(int x, int y)
		{
			throw new NotImplementedException();
		}

		public bool IsTransparent(int x, int y)
		{
			throw new NotImplementedException();
		}

		public bool IsWalkable(int x, int y)
		{
			throw new NotImplementedException();
		}

		public void Restore(MapState state)
		{
			throw new NotImplementedException();
		}

		public MapState Save()
		{
			throw new NotImplementedException();
		}

		public void SetCellProperties(int x, int y, bool isTransparent, bool isWalkable, bool isExplored)
		{
			throw new NotImplementedException();
		}

		public void SetCellProperties(int x, int y, bool isTransparent, bool isWalkable)
		{
			throw new NotImplementedException();
		}

		public string ToString(bool useFov)
		{
			throw new NotImplementedException();
		}
	}

}

