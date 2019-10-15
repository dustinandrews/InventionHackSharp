using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inv;

namespace Portable
{
	public static class Shell
	{
		public static void Install(Inv.Application application)
		{
			application.Title = "MegaDungeon";
			var surface = application.Window.NewSurface();
			var h = application.Window.Height;
			var w = application.Window.Width;
			InitializeSurface(surface);

			surface.ArrangeEvent += () =>
			{
				Surface_ArrangeEvent(surface);
			};

			application.Window.Transition(surface);
		}

		static void InitializeSurface(Surface surface)
		{
			var vericalCellCount = 20;
			var horizontalCellCount = 50;
			surface.Background.Colour = Colour.WhiteSmoke;
			var stack = surface.NewStack(Orientation.Vertical);
			surface.Content = stack;
			stack.Background.Colour = Colour.Green;
			stack.Size.Set(surface.Window.Width, surface.Window.Height);

			Label[,] labels = new Label[horizontalCellCount, vericalCellCount];


			var h = stack.Window.Height / vericalCellCount;
			var w = stack.Window.Width / horizontalCellCount;


			CreateCells(surface, vericalCellCount, horizontalCellCount, stack, w, h, labels);
			var engine = new MD.Engine(horizontalCellCount,vericalCellCount);
			for(int x = 0; x < horizontalCellCount; x++)
			{
				for(int y = 0; y < vericalCellCount; y++)
				{
					if(engine.Floor[x,y] == 1)
					{
						labels[x,y].Background.Colour = Colour.GraySmoke;
					}
					else
					{
						labels[x,y].Background.Colour = Colour.Black;
						// labels[x,y].Font.Colour = Colour.WhiteSmoke;
					}
				}

			}

		}

		static void CreateCells(Surface surface, int hcells, int wcells, Stack stack, int w, int h, Label[,] labels)
		{
			// var colorStep = 360.0 / (hcells * wcells);
			// var hue = 0.0;
			for (int i = 0; i < hcells; i++)
			{
				var hstack = surface.NewStack(Orientation.Horizontal);
				hstack.Size.SetHeight(h);
				hstack.Size.SetWidth(w * wcells);
				stack.AddPanel(hstack);
				for (int j = 0; j < wcells; j++)
				{
					var label = surface.NewLabel();
					label.Size.Set(w, h);
					labels[j, i] = label;
					hstack.AddPanel(label);
					// label.Text = $"{i},{j}";
					// label.Font.Size = 9;
					// label.Border.Colour = Colour.Black;
					// label.Border.Set(1);
					// label.Background.Colour = Colour.FromHSV(hue, 0.5, 0.5);
					// hue += colorStep;
				}
			}
		}

		static void Surface_ArrangeEvent(Surface surface)
		{
			var h = surface.Window.Height;
			var w = surface.Window.Width;
		}
	}
}
