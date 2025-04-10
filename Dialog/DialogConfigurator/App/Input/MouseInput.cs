using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DialogConfigurator.App.Input;

public class MouseInput
{
    private MouseState _currentMouseState;
    private MouseState _previousMouseState;

    public Point Position { get; internal set; }

    public void Update()
    {
        _previousMouseState = _currentMouseState;
        _currentMouseState = Mouse.GetState();
        Position = new Point(_currentMouseState.X, _currentMouseState.Y);
    }

    public static bool IsMouseButtonPressed(ButtonState current, ButtonState previous)
    {
        return current == ButtonState.Pressed && previous == ButtonState.Released;
    }

    public static bool IsMouseButtonHeld(ButtonState current, ButtonState previous)
    {
        return current == ButtonState.Pressed && previous == ButtonState.Pressed;
    }

    public static bool IsMouseButtonReleased(ButtonState current, ButtonState previous)
    {
        return current == ButtonState.Released && previous == ButtonState.Pressed;
    }

    public bool IsLeftButtonPressed() => IsMouseButtonPressed(_currentMouseState.LeftButton, _previousMouseState.LeftButton);
    public bool IsLeftButtonHeld() => IsMouseButtonHeld(_currentMouseState.LeftButton, _previousMouseState.LeftButton);
    public bool IsLeftButtonReleased() => IsMouseButtonReleased(_currentMouseState.LeftButton, _previousMouseState.LeftButton);

    public bool IsRightButtonPressed() => IsMouseButtonPressed(_currentMouseState.RightButton, _previousMouseState.RightButton);
    public bool IsRightButtonHeld() => IsMouseButtonHeld(_currentMouseState.RightButton, _previousMouseState.RightButton);
    public bool IsRightButtonReleased() => IsMouseButtonReleased(_currentMouseState.RightButton, _previousMouseState.RightButton);
}