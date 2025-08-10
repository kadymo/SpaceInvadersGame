using SpaceInvadersGame.Models;

namespace SpaceInvadersGame;

public class ObjectEventArgs
{
    public GameObject GameObject { get; set; }
    public Image ImageElement { get; set; }
    
    public ObjectEventArgs(GameObject gameObject)
    {
        GameObject = gameObject;
    }
    
    public ObjectEventArgs(Image imageElement)
    {
        ImageElement = imageElement;
    }
    
    public ObjectEventArgs(GameObject gameObject, Image imageElement)
    {
        GameObject = gameObject;
        ImageElement = imageElement;
    }
}
