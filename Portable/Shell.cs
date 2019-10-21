using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inv;

namespace Portable
{
	public static class Shell
	{
		static TimeSpan frameTime = TimeSpan.FromMilliseconds(8);
		public static void Install(Inv.Application application)
		{
			application.Title = "MegaDungeon";
			var surface = application.Window.NewSurface();
			var h = application.Window.Height;
			var w = application.Window.Width;
			var start = DateTime.UtcNow;
			var ui = new MegaDungeonUI(surface, 60, 40);
			var end = DateTime.UtcNow;
			var ms = (end - start).TotalMilliseconds;
			Console.WriteLine($"Initialized in {ms} milliseconds");

			surface.ArrangeEvent += () =>
			{
				Surface_ArrangeEvent(surface);
			};

			application.Window.Transition(surface);

			surface.ComposeEvent += () =>
			{
				Surface_ComposeEvent(ui);
			};

			surface.KeystrokeEvent += (keyStroke) =>
			{
				Surface_KeystrokeEvent(keyStroke, ui);
			};
			Console.WriteLine("Boot completed.");
		}

		static void Surface_KeystrokeEvent(Keystroke keystroke, MegaDungeonUI ui)
		{
			Console.WriteLine($"{keystroke.Modifier}{keystroke.Key}");
			ui.AcceptInput(keystroke);
		}
		static void Surface_ArrangeEvent(Surface surface)
		{

		}

		// Fires up to 60 times a second.
		static void Surface_ComposeEvent(MegaDungeonUI ui)
		{
		}
	}
}
