using Windows.System;
using SpaceInvadersGame.Models;

namespace SpaceInvadersGame;

public class GameManager
{
    private List<GameObject> _gameObjects = new List<GameObject>();
    private GameObject _player;
    
    private readonly InputManager _inputManager;
    
    private DateTime _lastUpdate;
    private bool _isGameRunning;
    
    public GameManager(InputManager inputManager)
    {
        _inputManager = inputManager;
    }

    public void AddGameObject(GameObject gameObject)
    {
        _gameObjects.Add(gameObject);
        if (gameObject.Model is Player playerModel)
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
        // playerModel.PositionX += playerModel.Speed * deltaTimeSeconds;
        
        // Process input
        if (_inputManager.isKeyPressed(VirtualKey.W) || _inputManager.isKeyPressed(VirtualKey.Up))
        {
            playerModel.PositionY -= playerModel.Speed * deltaTimeSeconds;
        }

        if (_inputManager.isKeyPressed(VirtualKey.S) || _inputManager.isKeyPressed(VirtualKey.Down))
        {
            playerModel.PositionY += playerModel.Speed * deltaTimeSeconds;
        }
        
        if (_inputManager.isKeyPressed(VirtualKey.A) || _inputManager.isKeyPressed(VirtualKey.Left)) 
        {
            playerModel.PositionX -= playerModel.Speed * deltaTimeSeconds;
        }

        if (_inputManager.isKeyPressed(VirtualKey.D) || _inputManager.isKeyPressed(VirtualKey.Right))
        {
            playerModel.PositionX += playerModel.Speed * deltaTimeSeconds;
        }
        
        // Update positions (player, enemies, projectiles, etc.)
        // Verify collisions
        // Update game state (ponctuation, life, etc.)
    }
    
}
