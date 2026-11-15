using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;

namespace MyApp.Android;

[Activity(
    Label = "Крутой_прокат228",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
}
