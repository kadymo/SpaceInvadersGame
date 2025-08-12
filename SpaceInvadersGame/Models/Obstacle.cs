using SpaceInvadersGame.Models.Interfaces;

namespace SpaceInvadersGame.Models;

public class Obstacle : IPositionable
{
    public double PositionX { get; set; }
    public double PositionY { get; set; }
    public double Health { get; set; } = 100;
}
