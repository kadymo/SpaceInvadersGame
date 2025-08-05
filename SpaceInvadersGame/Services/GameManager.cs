using System.Diagnostics.CodeAnalysis;
using Windows.System;
using Microsoft.UI.Xaml.Media.Imaging;
using SpaceInvadersGame.Models;

namespace SpaceInvadersGame;

public class GameManager
{
    private List<GameObject> _gameObjects;
    private GameObject _player;
    private GameObject _projectile;
    
    private readonly InputManager _inputManager;
    private readonly SoundManager _soundManager;

    private bool _canShoot = true;
    private DateTime _lastUpdate;
    private bool _isGameRunning;
    
    public int Score { get; set; }
    
    public event EventHandler<Projectile> ProjectileFired;
    public event EventHandler<GameObject> ProjectileExceededScreen;
    public event EventHandler<CollisionEventArgs> ProjectileHit;
    public event EventHandler<CollisionEventArgs> ObstacleHit;
    
    public GameManager(List<GameObject> gameObjects, InputManager inputManager, SoundManager soundManager)
    {
        _gameObjects = gameObjects;
        _inputManager = inputManager;
        _soundManager = soundManager;

        this.ProjectileFired += (sender, projectileModel) =>
        {
            _soundManager.PlaySound("ProjectileFiredSound.wav");
        };

        this.ProjectileExceededScreen += (sender, gameObject) =>
        {
            _canShoot = true;
        };

        this.ProjectileHit += (sender, collisionData) =>
        {
            Score += 100;
            _canShoot = true;
            _soundManager.PlaySound("ProjectileHitSound.wav");
        };
        
        this.ObstacleHit += (sender, collisionData) =>
        {
            _canShoot = true;
            _soundManager.PlaySound("ObstacleHitSound.wav");
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

    public void Start()
    {
        _isGameRunning = true;
        _lastUpdate = DateTime.Now;
    }

    public void Stop()
    {
        _isGameRunning = false;
    }
    
    public void Update(TimeSpan deltaTime)
    {
        if (!_isGameRunning) return;
        float deltaTimeSeconds = (float)deltaTime.TotalSeconds;
        
        var playerModel = _player.Model as Player;
        
        GameObject projectileGameObject = _gameObjects.Find(go => go.Model is Projectile);
        
        if (projectileGameObject != null && projectileGameObject.Model is Projectile projectileModel)
        {
            projectileModel.PositionY -= projectileModel.Speed * deltaTimeSeconds;
            
            if (projectileModel.PositionY + projectileGameObject.View.Height < 0)
            {
                ProjectileExceededScreen?.Invoke(this, projectileGameObject);
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

            ProjectileFired?.Invoke(this, projectileModel);
        }
        
        VerifyObstacleCollision();
        VerifyCollision();
    }

    private void VerifyCollision()
    {
        var projectileGameObject = _gameObjects.Find(go => go.Model is Projectile);
        
        if (projectileGameObject == null) return;
        
        for (int i = _gameObjects.Count - 1; i>= 0; i--)
        {
            var enemyGameObject = _gameObjects[i];
            if (projectileGameObject.Model is Projectile projectileModel && enemyGameObject.Model is Enemy enemyModel)
            {
                var heightCollisionCondition = projectileModel.PositionY + 10 < enemyModel.PositionY + enemyGameObject.View.Height;
              
                var widthCollisionCondition = projectileModel.PositionX + projectileGameObject.View.Width - 20 > enemyModel.PositionX 
                                              && projectileModel.PositionX - 10 < enemyModel.PositionX + enemyGameObject.View.Width;
                
                if (heightCollisionCondition && widthCollisionCondition)
                {
                    var collisionData = new CollisionEventArgs(projectileGameObject, enemyGameObject);
                    ProjectileHit?.Invoke(this, collisionData);
                    break;
                }
            }
        }
    }
    
    private void VerifyObstacleCollision()
    {
        var projectileGameObject = _gameObjects.Find(go => go.Model is Projectile);
        
        if (projectileGameObject == null) return;
        
        for (int i = _gameObjects.Count - 1; i>= 0; i--)
        {
            var obstacleGameObject = _gameObjects[i];
            if (projectileGameObject.Model is Projectile projectileModel && obstacleGameObject.Model is Obstacle obstacleModel)
            {
                var heightCollisionCondition = projectileModel.PositionY + 10 < obstacleModel.PositionY + obstacleGameObject.View.Height;
              
                var widthCollisionCondition = projectileModel.PositionX + projectileGameObject.View.Width - 20 > obstacleModel.PositionX 
                                              && projectileModel.PositionX - 10 < obstacleModel.PositionX + obstacleGameObject.View.Width;
                
                if (heightCollisionCondition && widthCollisionCondition)
                {
                    var collisionData = new CollisionEventArgs(projectileGameObject, obstacleGameObject);
                    ObstacleHit?.Invoke(this, collisionData);
                    break;
                }
            }
        }
    }
}
