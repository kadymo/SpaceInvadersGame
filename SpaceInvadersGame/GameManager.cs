using System.Diagnostics.CodeAnalysis;
using Windows.System;
using Microsoft.UI.Xaml.Media.Imaging;
using SpaceInvadersGame.Models;

namespace SpaceInvadersGame;

public class GameManager
{
    private List<GameObject> _gameObjects = new List<GameObject>();
    private GameObject _player;
    private GameObject _projectile;
    
    private readonly InputManager _inputManager;
    
    private float _screenWidth;
    private float _screenHeight;
    private bool _canShoot = true;
    
    private DateTime _lastUpdate;
    private bool _isGameRunning;
    
    public event EventHandler<Projectile> OnProjectileFired;
    public event EventHandler<GameObject> OnProjectileExceededScreen;
    
    public GameManager(InputManager inputManager)
    {
        _inputManager = inputManager;

        this.OnProjectileExceededScreen += (sender, gameObject) =>
        {
            _gameObjects.Remove(gameObject);
            _canShoot = true;
        };
    }

    public void AddGameObject(GameObject gameObject)
    {
        _gameObjects.Add(gameObject);
        if (gameObject.Model is Player playerModel)
        {
            _player = gameObject;
        }

        if (gameObject.Model is Projectile projectileModel)
        {
            _projectile = gameObject;
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
    
    // Method called every frame]
    

    public void SetScreenBounds(float width, float height)
    {
        _screenWidth = width;
        _screenHeight = height;
    }
    
    public void Update(TimeSpan deltaTime)
    {
        
        if (!_isGameRunning) return;
        float deltaTimeSeconds = (float)deltaTime.TotalSeconds;
        
        var playerModel = _player.Model as Player;
        // Projectile projectileModel = null;
        
        // Process input
        GameObject projectileGameObject = _gameObjects.Find(go => go.Model is Projectile);
        
        if (projectileGameObject != null && projectileGameObject.Model is Projectile projectileModel)
        {
            projectileModel.PositionY -= projectileModel.Speed * deltaTimeSeconds;
            
            if (projectileModel.PositionY + projectileGameObject.View.Height < 0)
            {
                OnProjectileExceededScreen?.Invoke(this, projectileGameObject);
                _gameObjects.Remove(projectileGameObject);
            }
        }
        
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

        if (_inputManager.isKeyPressed(VirtualKey.Space) && _canShoot)
        {
            _canShoot = false;
            
            projectileModel = new Projectile
            {
                PositionX = playerModel.PositionX,
                PositionY = playerModel.PositionY,
            };

            OnProjectileFired?.Invoke(this, projectileModel);
        }
        
        // Update positions (player, enemies, projectiles, etc.)
        // Verify collisions
        // Update game state (ponctuation, life, etc.)
    }
    
}
