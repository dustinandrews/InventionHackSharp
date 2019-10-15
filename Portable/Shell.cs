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
			application.Title = "My Project";
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
			var hcells = 10;
			var wcells = 20;
			surface.Background.Colour = Colour.WhiteSmoke;
			var stack = surface.NewStack(Orientation.Vertical);
			surface.Content = stack;
			stack.Background.Colour = Colour.Green;
			stack.Size.Set(surface.Window.Width, surface.Window.Height);

			Label[,] labels = new Label[hcells, wcells];


			var h = stack.Window.Height / hcells;
			var w = stack.Window.Width / wcells;


			CreateCells(surface, hcells, wcells, stack, w, h, labels);
		}

		static void CreateCells(Surface surface, int hcells, int wcells, Stack stack, int w, int h, Label[,] labels)
		{
			var colorStep = 360.0 / (hcells * wcells);
			var hue = 0.0;
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
					label.Text = $"{i},{j}";
					label.Font.Size = 9;
					label.Border.Colour = Colour.Black;
					label.Border.Set(1);
					label.Background.Colour = Colour.FromHSV(hue, 0.5, 0.5);
					hue += colorStep;
					labels[i, j] = label;
					hstack.AddPanel(label);
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
