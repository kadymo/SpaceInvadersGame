namespace SpaceInvadersGame.Models;

public class GameObject
{
    public Image View { get; set; }
    public object Model { get; set; }

    public RotateTransform Rotation { get; set; }
    
    public GameObject(Image view, object model)
    {
        View = view;
        Model = model;
    }
}
