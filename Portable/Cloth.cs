/*

	Cloth class by Callan Hodgskin 2019, Modified by Dustin Andrews 2019
	Original code https://gitlab.com/snippets/1905512

	Example:
	var Cloth = new Cloth();
	Cloth.Dimension = new Inv.Dimension(20, 20);
	Cloth.DrawEvent += (DC, Patch) =>
	{
		if ((Patch.X % 2) == (Patch.Y % 2))
			DC.DrawRectangle(Inv.Colour.DarkGray, null, 0, Patch.Rect);
		else
			DC.DrawRectangle(Inv.Colour.LightGray, null, 0, Patch.Rect);
	};
	Cloth.Draw();

 */

using System;

namespace Inv
{

	public class Patch
	{
		public int X;
		public int Y;
		public int CellX;
		public int CellY;
		public Rect Rect;

	}

	internal sealed class Cloth : Inv.Panel<Inv.Canvas>
	{
		public Cloth()
		{
			this.CellSizeProperty = 32;
			this.Patch = new Patch();

			this.Base = Inv.Canvas.New();
			Base.PressEvent += (Point) =>
			{
				this.PanningPoint = Point;

				Base.Draw();
			};

			Base.MoveEvent += (Point) =>
			{
				if (PanningPoint != null)
				{
					var DeltaPoint = PanningPoint.Value - Point;

					PanningX += DeltaPoint.X;
					PanningY += DeltaPoint.Y;

					this.PanningPoint = Point;

					Base.Draw();
				}
			};

			Base.ReleaseEvent += (Point) =>
			{
				PanningPoint = null;

				Base.Draw();
			};

			Base.ZoomEvent += (Z) =>
			{
				var GlobalX = PanningX + Z.Point.X;
				var GlobalY = PanningY + Z.Point.Y;
				var OldX = GlobalX / CellSizeProperty;
				var OldY = GlobalY / CellSizeProperty;
				var ModX = GlobalX % CellSizeProperty;
				var ModY = GlobalY % CellSizeProperty;

				CellSizeProperty += Z.Delta * (CellSize / 8);
				if (CellSizeProperty > 256)
					CellSizeProperty = 256;
				else if (CellSizeProperty < 4)
					CellSizeProperty = 4;
				PanningX = (OldX * CellSizeProperty) - Z.Point.X + ModX;
				PanningY = (OldY * CellSizeProperty) - Z.Point.Y + ModY;
				Base.Draw();
			};

			Base.DrawEvent += (DC) =>
			{
				var CanvasWidth = Base.Window.Width;
				var CanvasHeight = Base.Window.Height;
				var MapWidth = Dimension.Width * CellSizeProperty;
				var MapHeight = Dimension.Height * CellSizeProperty;
				var BufferX = Math.Max(300, CanvasWidth - MapWidth);
				var BufferY = Math.Max(300, CanvasHeight - MapHeight);

				if (InitialDraw)
				{
					this.InitialDraw = false;

					if (PanningX == 0 && PanningY == 0)
					{
						if (MapWidth < CanvasWidth)
							PanningX = (MapWidth - CanvasWidth) / 2;

						if (MapHeight < CanvasHeight)
							PanningY = (MapHeight - CanvasHeight) / 2;
					}
				}

				if (PanningX + CanvasWidth > MapWidth + BufferX)
					PanningX = MapWidth - CanvasWidth + BufferX;

				if (PanningX < -BufferX)
					PanningX = -BufferX;

				if (PanningY + CanvasHeight > MapHeight + BufferY)
					PanningY = MapHeight - CanvasHeight + BufferY;

				if (PanningY < -BufferY)
					PanningY = -BufferY;

				this.PanningWidth = CanvasWidth / CellSizeProperty;
				this.PanningHeight = CanvasHeight / CellSizeProperty;
				this.PanningLeft = PanningX / CellSizeProperty;
				this.PanningTop = PanningY / CellSizeProperty;
				this.PanningRight = PanningLeft + PanningWidth + 1;
				this.PanningBottom = PanningTop + PanningHeight + 1;

				var CellY = -(PanningY % CellSizeProperty);

				for (var Y = PanningTop; Y <= PanningBottom; Y++)
				{
					var CellX = -(PanningX % CellSizeProperty);

					for (var X = PanningLeft; X <= PanningRight; X++)
					{
						if (X >= 0 && X < Dimension.Width && Y >= 0 && Y < Dimension.Height)
						{
							Patch.X = X;
							Patch.Y = Y;
							Patch.Rect = new Inv.Rect(CellX, CellY, CellSizeProperty, CellSizeProperty);

							DrawEvent?.Invoke(DC, Patch);
						}

						CellX += CellSizeProperty;
					}

					CellY += CellSizeProperty;
				}
			};
		}

		public Inv.Dimension Dimension { get; set; }
		public int CellSize { get => CellSizeProperty; set => CellSizeProperty = value; }

		public event Action<Inv.DrawContract, Patch> DrawEvent;

		public void Draw() => Base.Draw();

		private bool InitialDraw;
		private int CellSizeProperty;
		private int PanningX;
		private int PanningY;
		private Inv.Point? PanningPoint;
		private int PanningWidth;
		private int PanningHeight;
		private int PanningLeft;
		private int PanningTop;
		private int PanningRight;
		private int PanningBottom;
		private Patch Patch;
	}
}