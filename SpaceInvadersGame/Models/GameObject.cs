using SpaceInvadersGame.Models.Interfaces;

namespace SpaceInvadersGame.Models;

public abstract class GameObject
{
    public FrameworkElement View { get; set; }
    public object Model { get; set; }

    public RotateTransform Rotation { get; set; }

    public abstract IPositionable GetPositionable();
    
    public void UpdateViewPosition()
    {
        var positionable = GetPositionable();
        Canvas.SetLeft(View, positionable.PositionX);
        Canvas.SetTop(View, positionable.PositionY);
    }
    
    protected GameObject(FrameworkElement view)
    {
        View = view;
    }
}

public class GameObject<T> : GameObject where T : class, IPositionable
{
    public T Model { get; set; }
    
    public GameObject(FrameworkElement view, T model) : base(view)
    {
        Model = model;
    }
    
    public override IPositionable GetPositionable() => Model;

}

public class PlayerGameObject : GameObject<Player>
{
    public PlayerGameObject(FrameworkElement view, Player model) : base(view, model)
    {
    }
}

public class EnemyGameObject : GameObject<Enemy>
{
    public EnemyGameObject(FrameworkElement view, Enemy model) : base(view, model) { }
}

public class ProjectileGameObject : GameObject<Projectile>
{
    public ProjectileGameObject(FrameworkElement view, Projectile model) : base(view, model) { }
}

public class ObstacleGameObject : GameObject<Obstacle>
{
    public ObstacleGameObject(FrameworkElement view, Obstacle model) : base(view, model) { }
}

