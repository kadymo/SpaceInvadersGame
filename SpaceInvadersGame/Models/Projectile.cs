using SpaceInvadersGame.Models.Enums;
using SpaceInvadersGame.Models.Interfaces;

namespace SpaceInvadersGame.Models;

public class Projectile : IPositionable
{
    public double PositionX { get; set; }
    public double PositionY { get; set; }
    public double Health { get; set; } = 100;
    public double Speed { get; set; } = 150;
    public ProjectileFirer Firer { get; set; }
}
