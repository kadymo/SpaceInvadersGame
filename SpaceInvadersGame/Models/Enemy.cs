namespace SpaceInvadersGame.Models;

public class Enemy
{
    public double PositionX { get; set; } = 0;
    public double PositionY { get; set; }
    public double Health { get; set; } = 100;
    public double Speed { get; set; } = 300;

    public event Action OnEnemyDied;
    
    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            Console.WriteLine("Player Died!");
            OnEnemyDied?.Invoke();
        }
    }
}
