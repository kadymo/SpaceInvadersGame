using SpaceInvadersGame.Models;

namespace SpaceInvadersGame;

public class GameManager
{
    private List<GameObject> _gameObjects;
    private GameObject _player;
    
    private readonly Canvas _gameCanvas;
    
    private DateTime _lastUpdate;
    private bool _isGameRunning;
    
    public GameManager(Canvas gameCanvas)
    {
        _gameCanvas = gameCanvas;
    }

    public void AddGameObject(GameObject gameObject)
    {
        _gameObjects.Add(gameObject);
        if (gameObject.Model is Player playerMode)
        {
            _player = gameObject;
        }
    }
    
    public void HandlePlayerDied()
    {
        Console.WriteLine("Displaying Game Over screen.");
    }

    public void Start()
    {
        _isGameRunning = true;
        _lastUpdate = DateTime.Now;
        // lógica inicial 
    }

    public void Stop()
    {
        _isGameRunning = false;
    }
    
    // Method called every frame
    public void Update(TimeSpan deltaTime)
    {
        if (!_isGameRunning) return;
        float deltaTimeSeconds = (float)deltaTime.TotalSeconds;
        
        var playerModel = _player.Model as Player;
        playerModel.PositionX += playerModel.Speed * deltaTimeSeconds;
        
        // Process input
        // Update positions (player, enemies, projectiles, etc.)
        // Verify collisions
        // Update game state (ponctuation, life, etc.)
    }
    
}
