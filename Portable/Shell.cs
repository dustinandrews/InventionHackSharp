﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inv;

namespace Portable
{
	public static partial class Shell
	{
		static TimeSpan frameTime = TimeSpan.FromMilliseconds(8);
		public static void Install(Inv.Application application)
		{
			application.Title = "MegaDungeon";
			var surface = application.Window.NewSurface();
			var h = application.Window.Height;
			var w = application.Window.Width;
			var start = DateTime.UtcNow;
			var ui = new MegaDungeonUI(surface, 35, 40);
			ui.InitializeSurface(surface);
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
		}


		static void Surface_ArrangeEvent(Surface surface)
		{
		}

		// Fires up to 60 times a second.
		static bool toggle = true;
		static void Surface_ComposeEvent(MegaDungeonUI ui)
		{
			if(toggle)
			{
				var start = DateTime.UtcNow;
				var end = start + frameTime;
				var now = DateTime.UtcNow;
				do{
					ui.Update();
					now = DateTime.UtcNow;
				}while(now < end);
			}
			// toggle = !toggle;
		}
	}
}
