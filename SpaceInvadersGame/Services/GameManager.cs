using Windows.Foundation;
using Windows.System;
using SpaceInvadersGame.Models;
using SpaceInvadersGame.Models.Enums;

namespace SpaceInvadersGame;

public class GameManager
{
    private List<GameObject> _gameObjects;
    
    private readonly InputManager _inputManager;
    private readonly SoundManager _soundManager;
    
    private DispatcherTimer _enemyProjectileTimer;
    private DispatcherTimer _specialEnemyTimer;
    private DateTime _lastUpdate;

    private bool _canShoot = true;
    public bool CanShoot => _canShoot;
    
    private bool _isGameRunning;
    public bool IsGameRunning => _isGameRunning;
    
    private IEnumerable<PlayerGameObject> GetPlayers() => _gameObjects.OfType<PlayerGameObject>();
    private IEnumerable<EnemyGameObject> GetEnemies() => _gameObjects.OfType<EnemyGameObject>().Where(e => e.Model.Type != EnemyType.SPECIAL);
    private IEnumerable<EnemyGameObject> GetSpecialEnemies() => _gameObjects.OfType<EnemyGameObject>().Where(e => e.Model.Type == EnemyType.SPECIAL);
    private IEnumerable<ProjectileGameObject> GetProjectiles() => _gameObjects.OfType<ProjectileGameObject>();
    private IEnumerable<ProjectileGameObject> GetPlayerProjectiles() => GetProjectiles().Where(p => p.Model.Firer == ProjectileFirer.PLAYER);
    private IEnumerable<ProjectileGameObject> GetEnemyProjectiles() => GetProjectiles().Where(p => p.Model.Firer == ProjectileFirer.ENEMY);
    private IEnumerable<ObstacleGameObject> GetObstacles() => _gameObjects.OfType<ObstacleGameObject>();
    
    public event EventHandler<Projectile> ProjectileFired;
    public event EventHandler<ProjectileGameObject> ProjectileExceededScreen;
    public event EventHandler<CollisionEventArgs> ProjectileHitEnemy;
    public event EventHandler<CollisionEventArgs> ProjectileHitPlayer;
    public event EventHandler<CollisionEventArgs> ObstacleHit;
    public event Action SpecialEnemyGenerated;
    public event EventHandler<EnemyGameObject> SpecialEnemyDestroyed;
    public event Action LifesIncreased;
    public event Action WaveEnd;
    public event EventHandler<GameOverEventArgs> GameOver;

    private double _swarmDirection = 1.0f;
    
    public int Score => GetPlayers().ToList().First().Model.Score;
    public int Lifes => GetPlayers().ToList().First().Model.Lifes;
    public int Wave = 1;
    
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

        this.ProjectileHitEnemy += (sender, collisionData) =>
        {
            var player = GetPlayers().ToList().First();
            var previousScore = player.Model.Score;
            
            switch (collisionData.EnemyTarget.Model.Type) 
            { 
                case EnemyType.LOW: 
                    player.Model.Score += 10; 
                    break;
                case EnemyType.MEDIUM: 
                    player.Model.Score += 20; 
                    break;
                case EnemyType.HIGH: 
                    player.Model.Score += 40; 
                    break;
                case EnemyType.SPECIAL:
                    player.Model.Score += 120;
                    break;
                }

            var previousScoreDouble = (double)previousScore / 1000;
            var scoreDouble = (double)player.Model.Score / 1000;
            
            if (Math.Floor(scoreDouble) > Math.Floor(previousScoreDouble) && player.Model.Lifes < 6)
            {
                player.Model.Lifes++;
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
        SetupSpecialEnemyTimer();
    }

    public void Stop()
    {
        _isGameRunning = false;
        _enemyProjectileTimer.Stop();
        _specialEnemyTimer.Stop();
    }

    private void SetupEnemyProjectileTimer()
    {
        _enemyProjectileTimer = new DispatcherTimer();
        _enemyProjectileTimer.Interval = TimeSpan.FromSeconds(1.5);
        _enemyProjectileTimer.Tick += ((sender, o) => FireEnemyProjectile());
        _enemyProjectileTimer.Start();
    }
    
    private void SetupSpecialEnemyTimer()
    {
        _specialEnemyTimer = new DispatcherTimer();
        _specialEnemyTimer.Interval = TimeSpan.FromSeconds(30);
        _specialEnemyTimer.Tick += ((sender, o) => GenerateSpecialEnemy());
        _specialEnemyTimer.Start();
    }
    
    public void Update(TimeSpan deltaTime, Rect bounds)
    {
        if (!_isGameRunning) return;
        
        float deltaTimeSeconds = (float)deltaTime.TotalSeconds;
        
        CheckGameOver();
        
        // Movement
        MovementPlayer(deltaTimeSeconds);
        MovementPlayerProjectile(deltaTimeSeconds);
        
        MovementEnemies(deltaTimeSeconds, bounds);
        MovementSpecialEnemies(deltaTimeSeconds, bounds);
        MovementEnemyProjectile(deltaTimeSeconds, bounds);
        
        // Collisions
        VerifyEnemiesCollision();
        VerifySpecialEnemiesCollision();
        VerifyPlayerCollision();
        VerifyObstacleCollision();
        
        // Fires
        FirePlayerProjectile();
        
        // Wave End
        CheckWaveEnd();
        
        _lastUpdate = DateTime.Now;
    }
    
    // Collisions
    
    private bool CheckCollision(ProjectileGameObject projectile, GameObject target)
    {
        var projectileBounds = GetObjectBounds(projectile);
        var targetBounds = GetObjectBounds(target);
        
        projectileBounds.Intersect(targetBounds);
        
        return !projectileBounds.IsEmpty;
    }

    private void VerifyEnemiesCollision()
    {
        var playerProjectiles = GetPlayerProjectiles().ToList();
        var enemies = GetEnemies().ToList();

        foreach (var projectile in playerProjectiles)
        {
            foreach (var enemy in enemies)
            {
                if (CheckCollision(projectile, enemy))
                {
                    var collisionData = new CollisionEventArgs(projectile, enemy);
                    ProjectileHitEnemy?.Invoke(this, collisionData);
                    break;
                }            
            }
        }
    }
    
    private void VerifySpecialEnemiesCollision()
    {
        var playerProjectiles = GetPlayerProjectiles().ToList();
        var enemies = GetSpecialEnemies().ToList();

        foreach (var projectile in playerProjectiles)
        {
            foreach (var enemy in enemies)
            {
                if (CheckCollision(projectile, enemy))
                {
                    var collisionData = new CollisionEventArgs(projectile, enemy);
                    ProjectileHitEnemy?.Invoke(this, collisionData);
                    break;
                }            
            }
        }
    }

    private void VerifyPlayerCollision()
    {
        if (!GetPlayers().ToList().Any()) return;
        var player = GetPlayers().ToList().First();
        
        var enemyProjectiles = GetEnemyProjectiles().ToList();

        foreach (var projectile in enemyProjectiles)
        {
            if (CheckCollision(projectile, player)) 
            { 
                var collisionData = new CollisionEventArgs(projectile, player); 
                ProjectileHitPlayer?.Invoke(this, collisionData);
                break;
            }   
        }
    }
    
    private void VerifyObstacleCollision()
    {
        var obstacles = GetObstacles().ToList();
        
        var playerProjectiles = GetPlayerProjectiles().ToList();
        foreach (var projectile in playerProjectiles)
        {
            foreach (var obstacle in obstacles)
            {
                if (CheckCollision(projectile, obstacle))
                {
                    var collisionData = new CollisionEventArgs(projectile, obstacle); 
                    ObstacleHit?.Invoke(this, collisionData);
                    break;
                }
            }        
        }
        
        var enemyProjectiles = GetEnemyProjectiles().ToList();
        foreach (var projectile in enemyProjectiles)
        {
            foreach (var obstacle in obstacles)
            {
                if (CheckCollision(projectile, obstacle))
                {
                    var collisionData = new CollisionEventArgs(projectile, obstacle); 
                    ObstacleHit?.Invoke(this, collisionData);
                    break;
                }
            }
        }
    }

    private void CheckWaveEnd()
    {
        var enemies = GetEnemies().ToList();
        if (enemies.Count == 0)
        {
            Wave += 1;
            WaveEnd.Invoke();
        }
    }
    
    private Rect GetObjectBounds(GameObject gameObject)
    {
        double x = 0, y = 0; 
        switch (gameObject)
        {
            case PlayerGameObject player:
                x = player.Model.PositionX;
                y = player.Model.PositionY;
                break;
            case EnemyGameObject enemy:
                x = enemy.Model.PositionX;
                y = enemy.Model.PositionY;
                break;
            case ProjectileGameObject projectile:
                x = projectile.Model.PositionX;
                y = projectile.Model.PositionY;
                break;
            case ObstacleGameObject obstacle:
                x = obstacle.Model.PositionX;
                y = obstacle.Model.PositionY;
                break;
        }

        return new Rect(x, y, gameObject.View.Width, gameObject.View.Height);

    }
    
    // Movement
    
    private void MovementPlayer(float deltaTimeSeconds)
    {
        if (!GetPlayers().ToList().Any()) return;
        var player = GetPlayers().ToList().First();
        
        if (_inputManager.isKeyPressed(VirtualKey.W) || _inputManager.isKeyPressed(VirtualKey.Up))
        {
            player.Model.PositionY -= player.Model.Speed * deltaTimeSeconds;
        }

        if (_inputManager.isKeyPressed(VirtualKey.S) || _inputManager.isKeyPressed(VirtualKey.Down))
        {
            player.Model.PositionY += player.Model.Speed * deltaTimeSeconds;
        }
        
        if (_inputManager.isKeyPressed(VirtualKey.A) || _inputManager.isKeyPressed(VirtualKey.Left)) 
        {
            player.Model.PositionX -= player.Model.Speed * deltaTimeSeconds;
        }

        if (_inputManager.isKeyPressed(VirtualKey.D) || _inputManager.isKeyPressed(VirtualKey.Right))
        {
            player.Model.PositionX += player.Model.Speed * deltaTimeSeconds;
        }   
    }

    private void MovementEnemies(float deltaTimeSeconds, Rect bounds)
    {
        var enemies = GetEnemies().ToList();
        bool hitEdge = false;
        foreach (var enemy in enemies) enemy.Model.PositionX += enemy.Model.Speed * _swarmDirection * deltaTimeSeconds;
        
        double maxX = enemies.Max(enemy => enemy.Model.PositionX + enemy.View.Width);
        double minX = enemies.Min(enemy => enemy.Model.PositionX);

        if (maxX > bounds.Right)
        {
            double correction = maxX - bounds.Right;
            foreach (var enemy in enemies)
            {
                enemy.Model.PositionX -= correction;
                enemy.Model.Speed += 1;
            }
            
            _swarmDirection = -1.0f;
            hitEdge = true;
        }
        else if (minX < bounds.Left)
        {
            double correction = bounds.Left - minX;
            foreach (var enemy in enemies)
            {
                enemy.Model.PositionX += correction;
                enemy.Model.Speed += 1;
            }
            
            _swarmDirection = 1.0f;
            hitEdge = true;
        } 
        
        if (hitEdge)
        {
            foreach (var enemy in enemies) enemy.Model.PositionY += 1;
        }
    }

    private void MovementSpecialEnemies(float deltaTimeSeconds, Rect bounds)
    {
        var specialEnemies = GetSpecialEnemies().ToList();
        if (!specialEnemies.Any()) return;   
        
        var specialEnemy = specialEnemies.First();
        specialEnemy.Model.PositionX -= specialEnemy.Model.Speed * deltaTimeSeconds;

        if (specialEnemy.Model.PositionX + 75 < bounds.Left)
        {
            SpecialEnemyDestroyed.Invoke(this, specialEnemy);
        }
    }

    private void MovementEnemyProjectile(float deltaTimeSeconds, Rect bounds)
    {
        var enemyProjectiles = GetEnemyProjectiles().ToList();
        
        foreach (var projectile in enemyProjectiles)
        {
            projectile.Model.PositionY += projectile.Model.Speed * deltaTimeSeconds;
            
            if (projectile.Model.PositionY + projectile.View.Height > bounds.Bottom + 100) 
            { 
                ProjectileExceededScreen?.Invoke(this, projectile); 
                _gameObjects.Remove(projectile);
            }    
        }
        
    }
    
    private void MovementPlayerProjectile(float deltaTimeSeconds)
    {
        var playerProjectiles = GetPlayerProjectiles().ToList();

        foreach (var projectile in playerProjectiles)
        {
            projectile.Model.PositionY -= projectile.Model.Speed * deltaTimeSeconds;
            
            if (projectile.Model.PositionY + projectile.View.Height < 0) 
            { 
                ProjectileExceededScreen?.Invoke(this, projectile); 
                _gameObjects.Remove(projectile);
            }   
        }
    }
    
    // Fires
    
    private void FirePlayerProjectile()
    {
        if (!GetPlayers().ToList().Any()) return;
        var player = GetPlayers().ToList().First();
        
        if (_inputManager.isKeyPressed(VirtualKey.Space) && _canShoot)
        {
            if (!_canShoot) return;
            _canShoot = false;
            
            var projectileModel = new Projectile
            {
                PositionX = player.Model.PositionX,
                PositionY = player.Model.PositionY,
            };

            ProjectileFired?.Invoke(this, projectileModel);
        }
    }
    
    private void FireEnemyProjectile()
    {
        var highEnemies = GetEnemies().Where(x => x.Model.Type == EnemyType.HIGH).ToList();
        if (!highEnemies.Any()) return;    
        
        Random random = new Random();
        var enemyIndex = random.Next(0, highEnemies.Count());

        var projectileModel = new Projectile 
        { 
            PositionX = highEnemies.ElementAt(enemyIndex).Model.PositionX, 
            PositionY = highEnemies.ElementAt(enemyIndex).Model.PositionY, 
            Firer = ProjectileFirer.ENEMY 
            
        };
        ProjectileFired?.Invoke(this, projectileModel); 
        
    }
    
    private void GenerateSpecialEnemy()
    {
        SpecialEnemyGenerated.Invoke();
    }

    private void CheckGameOver()
    {
        var players = GetPlayers().ToList();
        
        if (!players.Any() || players.First().Model.Lifes <= 0)
        {
            if (_isGameRunning)
            {
                GameOver.Invoke(this, new GameOverEventArgs { Score = Score });
                Stop();                
            }
        }
    }
}
