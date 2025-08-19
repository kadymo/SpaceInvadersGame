using Windows.Foundation;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using SpaceInvadersGame.Models;
using SpaceInvadersGame.Models.Enums;

namespace SpaceInvadersGame.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly GameManager _gameManager;
    private readonly InputManager _inputManager;
    private readonly SoundManager _soundManager;
    
    public InputManager InputManager => _inputManager;
    
    [ObservableProperty]
    private string _score;
    
    [ObservableProperty]
    private string _wave;

    [ObservableProperty]
    private string _username;
    
    private readonly List<GameObject> _gameObjects = new List<GameObject>();
    public List<GameObject> GameObjects => _gameObjects;
    
    public MainViewModel()
    {
        _inputManager = new InputManager();
        _soundManager = new SoundManager();
        _gameManager = new GameManager(_gameObjects, _inputManager, _soundManager);
        
        _gameManager.ProjectileFired += OnProjectileFired;
        _gameManager.ProjectileHitEnemy += OnProjectileHitEnemy;
        _gameManager.ProjectileHitPlayer += OnProjectileHitPlayer;
        _gameManager.ProjectileExceededScreen += OnProjectileExeededScreen;
        _gameManager.ObstacleHit += OnObstacleHit;
        _gameManager.WaveEnd += OnWaveEnd;
        _gameManager.GameOver += OnGameOver;
    }

    public event EventHandler<PlayerGameObject> PlayerCreated;
    public event EventHandler<PlayerGameObject> PlayerRemoved;
    
    public event EventHandler<EnemyGameObject> EnemyCreated;
    public event EventHandler<EnemyGameObject> EnemyRemoved;
    
    public event EventHandler<ProjectileGameObject> ProjectileCreated;
    public event EventHandler<ProjectileGameObject> ProjectileRemoved;
    
    public event EventHandler<ObstacleGameObject> ObstacleCreated;
    public event EventHandler<ObstacleGameObject> ObstacleRemoved;
    
    public event EventHandler<FrameworkElement> ExplosionEffectCreated;
    public event EventHandler<FrameworkElement> ExplosionEffectRemoved;

    public event EventHandler<GameOverEventArgs> GameOver;

    public void Start()
    {
        _gameManager.Start();
        
        CreatePlayer();
        CreateEnemy();
        CreateObstacle();
    }
    
    public void Stop()
    {
        _gameManager.Stop();
    }
    
    public void Update(TimeSpan deltaTime, Rect bounds)
    {
        if (!_gameManager.IsGameRunning) return;
        _gameManager.Update(deltaTime, bounds);
        
        foreach (var go in _gameObjects)
        {
            go.UpdateViewPosition();
        }
    }

    // Game Event Handlers
    
    private void OnProjectileFired(object? sender, Projectile projectileModel) => CreateProjectile(projectileModel);
    
    private async void OnProjectileHitEnemy(object? sender, CollisionEventArgs collisionData)
    {
        Score = "SCORE: " + _gameManager.Score;
        
        var projectile = collisionData.Projectile;
        var enemy = collisionData.EnemyTarget;

        var explosionLeft = Canvas.GetLeft(enemy.View);
        var explosionTop = Canvas.GetTop(enemy.View);
            
        _gameObjects.Remove(enemy);
        _gameManager.RemoveGameObject(enemy);
        
        _gameObjects.Remove(projectile);
        _gameManager.RemoveGameObject(projectile);
        
        EnemyRemoved?.Invoke(this, enemy);
        ProjectileRemoved?.Invoke(this, projectile);
           
        Image explosionImage = new Image
        {
            Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/Explosion.gif")),
            Width = 45,
            Height = 45,
        };

        Canvas.SetLeft(explosionImage, explosionLeft);
        Canvas.SetTop(explosionImage, explosionTop);

        ExplosionEffectCreated(this, explosionImage);
        await Task.Delay(500);
        ExplosionEffectRemoved(this, explosionImage);
    }

    private async void OnProjectileHitPlayer(object? sender, CollisionEventArgs collisionData)
    {
        var projectile = collisionData.Projectile;
        var player = collisionData.PlayerTarget;

        _gameObjects.Remove(projectile);
        _gameManager.RemoveGameObject(projectile);
        ProjectileRemoved?.Invoke(this, projectile);
        
        player.Model.Lifes -= 1;
        if (player.Model.Lifes <= 0)
        { 
            _gameObjects.Remove(player);
            _gameManager.RemoveGameObject(player);

            PlayerRemoved?.Invoke(this, player);
        }


        var explosionLeft = Canvas.GetLeft(player.View);
        var explosionTop = Canvas.GetTop(player.View);
        
        Image explosionImage = new Image
        {
            Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/Explosion.gif")),
            Width = 45,
            Height = 45,
        };

        Canvas.SetLeft(explosionImage, explosionLeft);
        Canvas.SetTop(explosionImage, explosionTop);

        ExplosionEffectCreated(this, explosionImage);
        await Task.Delay(500);
        ExplosionEffectRemoved(this, explosionImage);
    }

    private void OnProjectileExeededScreen(object? sender, ProjectileGameObject projectile)
    {
        _gameObjects.Remove(projectile);
        ProjectileRemoved?.Invoke(this, projectile);
    }
    
    private void OnObstacleHit(object? sender, CollisionEventArgs collisionData)
    {
        var projectile = collisionData.Projectile;
        var obstacle = collisionData.ObstacleTarget;

        obstacle.Model.Health -= 5;
        
        if (obstacle.Model.Health <= 0) 
        { 
            _gameObjects.Remove(obstacle); 
            ObstacleRemoved?.Invoke(this, obstacle);
        }
                
        _gameObjects.Remove(projectile);
        
        ProjectileRemoved?.Invoke(this, projectile);
        
        obstacle.View.Opacity = (obstacle.Model.Health / 100);
    }

    private void OnWaveEnd()
    {
        Wave = "WAVE: " + _gameManager.Wave;
        CreateEnemy();
    }

    private void OnGameOver(object? sender, GameOverEventArgs e)
    {
        GameOver?.Invoke(this, e);
    }
    
    private void CreatePlayer()
    {
        Image playerImage = new Image
        {
            Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/Corinthians.png")),
            Width = 50,
            Height = 50,
        };

        PlayerGameObject player = new PlayerGameObject(playerImage, new Player());
        _gameObjects.Add(player);
        _gameManager.AddGameObject(player);
        
        PlayerCreated.Invoke(this, player);
    }
    
    // Game Objects Creation

    private void CreateEnemy()
    {
        const int columns = 11;
        const int rows = 5;
        const int enemyWidth = 40;
        const int enemyHeight = 40;
        const double spacing = 15;

        Uri GetEnemyImageUri(EnemyType enemyType)
        {
            switch (enemyType)
            {
                case EnemyType.LOW: return new Uri("ms-appx:///Assets/Images/EnemyLow.png");
                case EnemyType.MEDIUM: return new Uri("ms-appx:///Assets/Images/EnemyMedium.png");
                case EnemyType.HIGH: return new Uri("ms-appx:///Assets/Images/EnemyHigh.png");
                default: return new Uri("ms-appx:///Assets/Images/EnemyLow.png");
            }
        }
        
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Enemy enemyModel = new Enemy();
                if (i == 0) enemyModel.Type = EnemyType.HIGH;
                else if (i == 1 || i == 2) enemyModel.Type = EnemyType.MEDIUM;
                else enemyModel.Type = EnemyType.LOW;
                
                Image enemyImage = new Image
                {
                    Source = new BitmapImage(GetEnemyImageUri(enemyModel.Type)),
                    Width = 40,
                    Height = 40,
                };
                
                EnemyGameObject enemy = new EnemyGameObject(enemyImage, enemyModel);
                enemy.Model.PositionX = j * (enemyWidth + spacing); 
                enemy.Model.PositionY = i * (enemyHeight + spacing);
                
                _gameObjects.Add(enemy); 
                _gameManager.AddGameObject(enemy);
                
                EnemyCreated.Invoke(this, enemy);
            }
        }
    }

    private void CreateObstacle()
    {
        const int columns = 3;
        const int rows = 1;
        const int obstacleWidth = 150;
        const int obstacleHeight = 45;
        const double spacing = 150;
        
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Image obstacleImage = new Image
                {
                    Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/Obstacle.png")),
                    Width = 150,
                    Height = 45,
                };
                
                ObstacleGameObject obstacle = new ObstacleGameObject(obstacleImage, new Obstacle());
                
                obstacle.Model.PositionX = j * (obstacleWidth + spacing); 
                obstacle.Model.PositionY = i * (obstacleHeight + spacing) + 340;
                
                _gameObjects.Add(obstacle);
                _gameManager.AddGameObject(obstacle);
                
                ObstacleCreated.Invoke(this, obstacle);
            }
        }
    }

    private void CreateProjectile(Projectile projectileModel)
    {
        
        Image projectileImage = new Image
        {
            Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/Projectile.gif")),
            Width = 30,
            Height = 30,
        };
        
        var rotation = new RotateTransform();
        rotation.CenterX = projectileImage.Width / 2;
        rotation.CenterY = projectileImage.Height / 2;
        projectileImage.RenderTransform = rotation;
        
        ProjectileGameObject projectile = new ProjectileGameObject(projectileImage, projectileModel);
        projectile.Rotation = rotation;
        projectile.Rotation.Angle = projectileModel.Firer == ProjectileFirer.PLAYER ? 90 : 270;
        
        _gameObjects.Add(projectile);
        _gameManager.AddGameObject(projectile);

        ProjectileCreated.Invoke(this, projectile);
    }
}
