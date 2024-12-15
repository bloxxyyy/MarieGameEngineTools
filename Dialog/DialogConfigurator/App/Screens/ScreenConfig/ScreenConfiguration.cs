using System.Collections.Generic;

namespace YuiConfigurator.App.Screens.ScreenConfig;
public static class ScreenConfiguration
{
    #region Public Api
    public static void AlterCurrentScreen(EScreen screen) {
        CurrentScreen = screen;
        OnReset();
    }

    public static void Draw() => Screens[CurrentScreen].Draw();
    public static void Update() => Screens[CurrentScreen].Update();
    public static void OnReset() => Screens[CurrentScreen].OnReset();
    #endregion

    #region Internal Api
    private static EScreen CurrentScreen = EScreen.DialogPicker;

    private static readonly Dictionary<EScreen, IScreen> Screens = new() {
        { EScreen.DialogPicker, new DialogPicker() }
    };
    #endregion
}
