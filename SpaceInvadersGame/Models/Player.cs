using SpaceInvadersGame.Models.Interfaces;

namespace SpaceInvadersGame.Models;


public class Player : IPositionable
{
    public double PositionX { get; set; } = 300;
    public double PositionY { get; set; } = 450;
    public double Health { get; set; } = 100;
    public double Speed { get; set; } = 220;
    
    public int Lifes { get; set; } = 3;
    public int Score { get; set; } = 0;
}
