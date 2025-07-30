namespace SpaceInvadersGame.Models;

public class Projectile
{
    public double PositionX { get; set; }
    public double PositionY { get; set; }
    public double Health { get; set; } = 100;
    public double Speed { get; set; } = 300;
}
