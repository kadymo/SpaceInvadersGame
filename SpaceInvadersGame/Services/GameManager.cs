using System.Diagnostics.CodeAnalysis;
using Windows.Foundation;
using Windows.System;
using Microsoft.UI.Xaml.Media.Imaging;
using SpaceInvadersGame.Models;
using SpaceInvadersGame.Models.Enums;

namespace SpaceInvadersGame;

public class GameManager
{
    private List<GameObject> _gameObjects;
    private GameObject _player;
    
    private readonly InputManager _inputManager;
    private readonly SoundManager _soundManager;
    
    private DispatcherTimer _enemyProjectileTimer;

    private bool _canShoot = true;
    private DateTime _lastUpdate;
    private bool _isGameRunning;
    
    public int Score { get; set; }
    
    public event EventHandler<Projectile> ProjectileFired;
    public event EventHandler<GameObject> ProjectileExceededScreen;
    public event EventHandler<CollisionEventArgs> ProjectileHit;
    public event EventHandler<CollisionEventArgs> ProjectileHitPlayer;
    public event EventHandler<CollisionEventArgs> ObstacleHit;

    private double _swarmDirection = 1.0f;
    
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
            if (collisionData.TargetGameObject.Model is Enemy enemyModel)
            {
                switch (enemyModel.Type)
                {
                    case EnemyType.LOW:
                        Score += 10;
                        break;
                    case EnemyType.MEDIUM:
                        Score += 20;
                        break;
                    case EnemyType.HIGH:
                        Score += 40;
                        break;
                }
            }
            
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
    }
    
    public void RemoveGameObject(GameObject gameObject)
    {
        _gameObjects.Remove(gameObject);
    }

    public void Start()
    {
        _isGameRunning = true;
        _lastUpdate = DateTime.Now;
        SetupEnemyProjectileTimer();
    }

    public void Stop()
    {
        _isGameRunning = false;
        _enemyProjectileTimer.Stop();
    }

    private void SetupEnemyProjectileTimer()
    {
        _enemyProjectileTimer = new DispatcherTimer();
        _enemyProjectileTimer.Interval = TimeSpan.FromSeconds(2);
        _enemyProjectileTimer.Tick += ((sender, o) => FireEnemyProjectile());
        _enemyProjectileTimer.Start();
    }
    
    public void Update(TimeSpan deltaTime, Rect bounds)
    {
        if (!_isGameRunning) return;
        float deltaTimeSeconds = (float)deltaTime.TotalSeconds;
        
        var playerModel = _player.Model as Player;
        var playerGameObject = _gameObjects.Find(go => go.Model is Player);
        
        GameObject playerProjectileGameObject = _gameObjects.Find(go => go.Model is Projectile proj && proj.Firer == ProjectileFirer.PLAYER);
        var enemyProjectiles = _gameObjects.Where(go => go.Model is Projectile proj && proj.Firer == ProjectileFirer.ENEMY).ToList();
        
        MovementPlayerProjectile(playerProjectileGameObject, deltaTimeSeconds);
        
        MovementEnemies(deltaTimeSeconds, bounds);
        MovementPlayer(playerModel, deltaTimeSeconds);
        
        FirePlayerProjectile(playerModel);
        
        // Collisions
        
        VerifyObstacleCollision();
        VerifyCollision();
        foreach (var enemyProjectile in enemyProjectiles)
        {
            MovementEnemyProjectile(enemyProjectile, deltaTimeSeconds, bounds);
            VerifyPlayerCollision(playerGameObject, enemyProjectile);
        }
    }

    private void VerifyCollision()
    {
        var projectileGameObject = _gameObjects.Find(go => go.Model is Projectile proj && proj.Firer == ProjectileFirer.PLAYER);
        
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

    private void VerifyPlayerCollision(GameObject playerGameObject, GameObject projectileGameObject)
    {
        if (projectileGameObject == null) return;
        
        if (playerGameObject.Model is Player playerModel && projectileGameObject.Model is Projectile projectileModel) 
        { 
            var heightCollisionCondition = projectileModel.PositionY + projectileGameObject.View.Height > playerModel.PositionY;
            var widthCollisionCondition = projectileModel.PositionX + projectileGameObject.View.Width - 20 > playerModel.PositionX 
                                          && projectileModel.PositionX - 10 < playerModel.PositionX + _player.View.Width;

            if (heightCollisionCondition && widthCollisionCondition) 
            { 
                var collisionData = new CollisionEventArgs(projectileGameObject, _player); 
                ProjectileHitPlayer?.Invoke(this, collisionData);
            }
        }
    }
    
    private void VerifyObstacleCollision()
    {
        var playerProjectile = _gameObjects.Find(go => go.Model is Projectile proj && proj.Firer == ProjectileFirer.PLAYER);
        if (playerProjectile != null)
        {
            CheckProjectileObstacleCollision(playerProjectile, true);
        }
    
        var enemyProjectiles = _gameObjects.Where(go => go.Model is Projectile proj && proj.Firer == ProjectileFirer.ENEMY).ToList();
        foreach (var enemyProjectile in enemyProjectiles)
        {
            CheckProjectileObstacleCollision(enemyProjectile, false);
        }
    }

    
    private void CheckProjectileObstacleCollision(GameObject projectileGameObject, bool isPlayerProjectile)
    {
        for (int i = _gameObjects.Count - 1; i >= 0; i--)
        {
            var obstacleGameObject = _gameObjects[i];
            if (projectileGameObject.Model is Projectile projectileModel && obstacleGameObject.Model is Obstacle obstacleModel)
            {
                bool heightCollisionCondition;
            
                if (isPlayerProjectile)
                {
                    heightCollisionCondition = projectileModel.PositionY <= obstacleModel.PositionY + obstacleGameObject.View.Height &&
                                               projectileModel.PositionY + projectileGameObject.View.Height >= obstacleModel.PositionY;
                }
                else
                {
                    heightCollisionCondition = projectileModel.PositionY + projectileGameObject.View.Height >= obstacleModel.PositionY &&
                                               projectileModel.PositionY <= obstacleModel.PositionY + obstacleGameObject.View.Height;
                }
            
                bool widthCollisionCondition = projectileModel.PositionX + projectileGameObject.View.Width > obstacleModel.PositionX &&
                                              projectileModel.PositionX < obstacleModel.PositionX + obstacleGameObject.View.Width;
            
                if (heightCollisionCondition && widthCollisionCondition)
                {
                    var collisionData = new CollisionEventArgs(projectileGameObject, obstacleGameObject);
                    ObstacleHit?.Invoke(this, collisionData);
                    break;
                }
            }
        }
    }


    private void MovementPlayer(Player playerModel, float deltaTimeSeconds)
    {
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
    }

    private void MovementEnemies(float deltaTimeSeconds, Rect bounds)
    {
        var enemies = _gameObjects.Where(x => x.Model is Enemy).Select(x => x).ToList();
        bool hitEdge = false;
        foreach (var enemy in enemies)
        {
            if (enemy.Model is Enemy enemyModel)
            {
                enemyModel.PositionX += enemyModel.Speed * _swarmDirection * deltaTimeSeconds;
            }
        }
        
        double maxX = enemies.Max(enemy => ((Enemy)enemy.Model).PositionX + enemy.View.Width);
        double minX = enemies.Min(enemy => ((Enemy)enemy.Model).PositionX);

        if (maxX > bounds.Right)
        {
            double correction = maxX - bounds.Right;
            foreach (var enemy in enemies)
            {
                if (enemy.Model is Enemy enemyModel)
                {
                    enemyModel.PositionX -= correction;
                }
            }
            _swarmDirection = -1.0f;
            hitEdge = true;
        }
        else if (minX < bounds.Left)
        {
            double correction = bounds.Left - minX;
            foreach (var enemy in enemies)
            {
                if (enemy.Model is Enemy enemyModel)
                {
                    enemyModel.PositionX += correction;
                }
            }
            _swarmDirection = 1.0f;
            hitEdge = true;
        } 
        
        if (hitEdge)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.Model is Enemy enemyModel)
                {
                    enemyModel.PositionY += 1;
                }
            }
        }
    }

    private void MovementPlayerProjectile(GameObject projectileGameObject, float deltaTimeSeconds)
    {
        if (projectileGameObject != null && projectileGameObject.Model is Projectile projectileModel)
        {
            projectileModel.PositionY -= projectileModel.Speed * deltaTimeSeconds;
            
            if (projectileModel.PositionY + projectileGameObject.View.Height < 0)
            {
                ProjectileExceededScreen?.Invoke(this, projectileGameObject);
                _gameObjects.Remove(projectileGameObject);
            }
        }
    }

    private void FirePlayerProjectile(Player playerModel)
    {
        if (_inputManager.isKeyPressed(VirtualKey.Space) && _canShoot)
        {
            _canShoot = false;
            
            var projectileModel = new Projectile
            {
                PositionX = playerModel.PositionX,
                PositionY = playerModel.PositionY,
            };

            ProjectileFired?.Invoke(this, projectileModel);
        }
    }

    private void MovementEnemyProjectile(GameObject projectileGameObject, float deltaTimeSeconds, Rect bounds)
    {
        if (projectileGameObject != null && projectileGameObject.Model is Projectile projectileModel)
        {
            if (projectileModel.Firer != ProjectileFirer.ENEMY) return;

            projectileModel.PositionY += projectileModel.Speed * deltaTimeSeconds;
            
            if (projectileModel.PositionY + projectileGameObject.View.Height > bounds.Bottom)
            {
                ProjectileExceededScreen?.Invoke(this, projectileGameObject);
                _gameObjects.Remove(projectileGameObject);
            }
        } 
    }

    private void FireEnemyProjectile()
    {
        var highEnemies = _gameObjects.Where(x => x.Model is Enemy enemy && enemy.Type == EnemyType.HIGH).ToList();
        if (!highEnemies.Any()) return;    
        
        Random random = new Random();
        var enemyIndex = random.Next(0, highEnemies.Count());

        if (highEnemies.ElementAt(enemyIndex).Model is Enemy enemyModel)
        {
            var projectileModel = new Projectile
            {
                PositionX = enemyModel.PositionX,
                PositionY = enemyModel.PositionY,
                Firer = ProjectileFirer.ENEMY
            }; 
            
            
            ProjectileFired?.Invoke(this, projectileModel); 
        }

    }
}
