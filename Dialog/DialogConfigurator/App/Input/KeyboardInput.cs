using Microsoft.Xna.Framework.Input;

namespace DialogConfigurator.App.Input;

public class KeyboardInput
{
    private KeyboardState _currentKeyboardState;
    private KeyboardState _previousKeyboardState;

    public void Update()
    {
        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();
    }

    public bool IsKeyPressed(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
    }

    public bool IsKeyHeld(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyDown(key);
    }

    public bool IsKeyReleased(Keys key)
    {
        return !_currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyDown(key);
    }
}
