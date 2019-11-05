using System;

namespace InventionUiWpf
{
	public class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			Inv.WpfShell.CheckRequirements(
				() =>
				{
// #if DEBUG
//					Inv.WpfShell.Options.PreventDeviceEmulation = false;
//					Inv.WpfShell.Options.DeviceEmulation = Inv.WpfDeviceEmulation.iPad_Mini;
// #endif
					Inv.WpfShell.Options.FullScreenMode = false;
					Inv.WpfShell.Options.DefaultWindowWidth = 800;
					Inv.WpfShell.Options.DefaultWindowHeight = 600;
					Inv.WpfShell.Run(Portable.Shell.Install);
				});
		}
	}
}
