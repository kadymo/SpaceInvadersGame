using SpaceInvadersGame.Models;

namespace SpaceInvadersGame;

public class CollisionEventArgs
{
    public GameObject ProjectileGameObject { get; set; }
    public GameObject TargetGameObject { get; set; }

    public CollisionEventArgs(GameObject projectileGameObject, GameObject targetGameObject)
    {
        ProjectileGameObject = projectileGameObject;
        TargetGameObject = targetGameObject;
    }
}
