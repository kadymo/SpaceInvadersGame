using SpaceInvadersGame.Models.Enums;
using SpaceInvadersGame.Models.Interfaces;

namespace SpaceInvadersGame.Models;

public class Enemy : IPositionable
{
    public double PositionX { get; set; } = 0;
    public double PositionY { get; set; }
    public double Health { get; set; } = 100;
    public double Speed { get; set; } = 15;
    public double FireSpeed { get; set; } = 150;
    public EnemyType Type { get; set; }
}
