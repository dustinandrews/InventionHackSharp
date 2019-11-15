namespace InventionUiAndroid
{
  [Android.App.Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
  public class MainActivity : Inv.AndroidActivity
  {
    protected override void Install(Inv.Application Application)
    {
      Portable.Shell.Install(Application);
    }
  }
}