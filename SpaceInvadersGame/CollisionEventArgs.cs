using SpaceInvadersGame.Models;

namespace SpaceInvadersGame;

public class CollisionEventArgs
{
    public ProjectileGameObject Projectile { get; set; }
    public PlayerGameObject PlayerTarget { get; set; }
    public EnemyGameObject EnemyTarget { get; set; }
    public ObstacleGameObject ObstacleTarget { get; set; }

    
    public CollisionEventArgs(ProjectileGameObject projectile, PlayerGameObject player)
    {
        Projectile = projectile;
        PlayerTarget = player;
    }
    
    public CollisionEventArgs(ProjectileGameObject projectile, EnemyGameObject enemy)
    {
        Projectile = projectile;
        EnemyTarget = enemy;
    }
    
    public CollisionEventArgs(ProjectileGameObject projectile, ObstacleGameObject obstacle)
    {
        Projectile = projectile;
        ObstacleTarget = obstacle;
    }
}
