namespace SpaceInvadersGame.Models;


public class Player
{
    public double PositionX { get; set; }
    public double PositionY { get; set; }
    public double Health { get; set; } = 100;
    public double Speed { get; set; } = 200;
    
    public event Action OnPlayerDied;

    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            Console.WriteLine("Player Died!");
            OnPlayerDied?.Invoke();
        }
    }
}
