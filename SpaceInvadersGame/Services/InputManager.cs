using Windows.System;

namespace SpaceInvadersGame;

public class InputManager
{
    private readonly HashSet<VirtualKey> _pressedKeys = new HashSet<VirtualKey>();
    
    public void KeyUp(VirtualKey key)
    {
        _pressedKeys.Remove(key);
    }
    
    public void KeyDown(VirtualKey key)
    {
        _pressedKeys.Add(key);
    }

    public bool isKeyPressed(VirtualKey key)
    {
        return _pressedKeys.Contains(key);
    }
}
