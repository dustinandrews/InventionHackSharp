using UIKit;

namespace InventionUiiOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            Inv.iOSShell.Run(Portable.Shell.Install);
        }
    }
}