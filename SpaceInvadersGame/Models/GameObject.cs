namespace SpaceInvadersGame.Models;

public class GameObject
{
    public UIElement View { get; set; }
    public object Model { get; set; }

    public GameObject(UIElement view, object model)
    {
        View = view;
        Model = model;
    }
}
