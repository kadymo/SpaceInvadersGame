using Windows.Foundation;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using SpaceInvadersGame.Models;

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
    private string _username;
    
    private readonly List<GameObject> _gameObjects = new List<GameObject>();
    public List<GameObject> GameObjects => _gameObjects;
    
    public MainViewModel()
    {
        _inputManager = new InputManager();
        _soundManager = new SoundManager();
        _gameManager = new GameManager(_gameObjects, _inputManager, _soundManager);
        
        _gameManager.ProjectileFired += OnProjectileFired;
        _gameManager.ProjectileHit += OnProjectileHit;
        _gameManager.ProjectileExceededScreen += OnProjectileExeededScreen;
        _gameManager.ObstacleHit += OnObstacleHit;
    }

    public event EventHandler<ObjectEventArgs> PlayerCreated;
    public event EventHandler<ObjectEventArgs> EnemyCreated;
    public event EventHandler<ObjectEventArgs> EnemyRemoved;
    public event EventHandler<ObjectEventArgs> ProjectileCreated;
    public event EventHandler<ObjectEventArgs> ProjectileRemoved;
    public event EventHandler<ObjectEventArgs> ObstacleCreated;
    public event EventHandler<ObjectEventArgs> ObstacleRemoved;
    public event EventHandler<ObjectEventArgs> ExplosionEffectCreated;
    public event EventHandler<ObjectEventArgs> ExplosionEffectRemoved;
    public event EventHandler<SwarmMovedEventArgs> SwarmMoved;

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
        _gameManager.Update(deltaTime, bounds);
        
        foreach (var go in _gameObjects)
        {
            if (go.Model is Player player)
            {
                Canvas.SetLeft(go.View, player.PositionX);
                Canvas.SetTop(go.View, player.PositionY);
            }

            if (go.Model is Enemy enemy)
            {
                Canvas.SetLeft(go.View, enemy.PositionX);
                Canvas.SetTop(go.View, enemy.PositionY);
            }
            
            if (go.Model is Projectile projectile)
            {
                Canvas.SetLeft(go.View, projectile.PositionX);
                Canvas.SetTop(go.View, projectile.PositionY);;
            }
            
            if (go.Model is Obstacle obstacle)
            {
                Canvas.SetLeft(go.View, obstacle.PositionX);
                Canvas.SetTop(go.View, obstacle.PositionY);;
            }
        }
    }

    // Game Event Handlers
    
    private void OnProjectileFired(object? sender, Projectile projectileModel) => CreateProjectile(projectileModel);
    
    private async void OnProjectileHit(object? sender, CollisionEventArgs collisionData)
    {
        Score = "SCORE: " + _gameManager.Score;
        
        var enemyGameObject = collisionData.TargetGameObject;
        var projectileGameObject = collisionData.ProjectileGameObject;

        var explosionLeft = Canvas.GetLeft(enemyGameObject.View);
        var explosionTop = Canvas.GetTop(enemyGameObject.View);
            
        _gameObjects.Remove(enemyGameObject);
        _gameManager.RemoveGameObject(enemyGameObject);
        
        _gameObjects.Remove(projectileGameObject);
        _gameManager.RemoveGameObject(projectileGameObject);
            
        EnemyRemoved?.Invoke(this, new ObjectEventArgs(enemyGameObject, enemyGameObject.View));
        ProjectileRemoved?.Invoke(this, new ObjectEventArgs(projectileGameObject));
           
        Image explosionImage = new Image
        {
            Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/Explosion.gif")),
            Width = 45,
            Height = 45,
        };

        Canvas.SetLeft(explosionImage, explosionLeft);
        Canvas.SetTop(explosionImage, explosionTop);


        ExplosionEffectCreated(this, new ObjectEventArgs(explosionImage));
        await Task.Delay(500);
        ExplosionEffectRemoved(this, new ObjectEventArgs(explosionImage));
    }

    private void OnProjectileExeededScreen(object? sender, GameObject gameObject)
    {
        _gameObjects.Remove(gameObject);
        ProjectileRemoved?.Invoke(this, new ObjectEventArgs(gameObject));
    }
    
    private void OnObstacleHit(object? sender, CollisionEventArgs collisionData)
    {
        var obstacleGameObject = collisionData.TargetGameObject;
        var projectileGameObject = collisionData.ProjectileGameObject;

        if (obstacleGameObject.Model is Obstacle obstacleModel)
        {
            obstacleModel.Health -= 5;
                
            if (obstacleModel.Health <= 0)
            {
                _gameObjects.Remove(obstacleGameObject);
                ObstacleRemoved?.Invoke(this, new ObjectEventArgs(obstacleGameObject));
            }
                
            _gameObjects.Remove(projectileGameObject); 
            
            ProjectileRemoved?.Invoke(this, new ObjectEventArgs(projectileGameObject));    
                
            obstacleGameObject.View.Opacity = (obstacleModel.Health / 100);
        }
    }
    
    private void CreatePlayer()
    {
        Image playerImage = new Image
        {
            Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/Corinthians.png")),
            Width = 50,
            Height = 50,
        };

        GameObject playerGameObject = new GameObject(playerImage, new Player());
        _gameObjects.Add(playerGameObject);
        _gameManager.AddGameObject(playerGameObject);
        
        var eventData = new ObjectEventArgs(playerGameObject, playerImage);
        PlayerCreated.Invoke(this, eventData);
    }
    
    // Game Objects Creation

    private void CreateEnemy()
    {
        const int columns = 6;
        const int rows = 4;
        const int enemyWidth = 50;
        const int enemyHeight = 50;
        const double spacing = 25;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Image enemyImage = new Image
                {
                    Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/Palmeiras.svg.png")),
                    Width = 45,
                    Height = 45,
                };
                
                GameObject enemyGameObject = new GameObject(enemyImage, new Enemy());
                
                if (enemyGameObject.Model is Enemy enemyModel)
                {
                    enemyModel.PositionX = j * (enemyWidth + spacing);
                    enemyModel.PositionY = i * (enemyHeight + spacing);
                }
                
                _gameObjects.Add(enemyGameObject); 
                _gameManager.AddGameObject(enemyGameObject);
                
                var eventData = new ObjectEventArgs(enemyGameObject, enemyImage);
                EnemyCreated.Invoke(this, eventData);
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
                
                GameObject obstacleGameObject = new GameObject(obstacleImage, new Obstacle());
                
                if (obstacleGameObject.Model is Obstacle obstacleModel)
                {
                    obstacleModel.PositionX = j * (obstacleWidth + spacing);
                    obstacleModel.PositionY = i * (obstacleHeight + spacing) + 340;
                }
                _gameObjects.Add(obstacleGameObject);
                _gameManager.AddGameObject(obstacleGameObject);
                
                var eventData = new ObjectEventArgs(obstacleGameObject, obstacleImage);
                ObstacleCreated.Invoke(this, eventData);
            }
        }
    }

    private void CreateProjectile(Projectile projectileModel)
    {
        Image projectileImage = new Image
        {
            Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/Projectile.gif")),
            Width = 20,
            Height = 20,
        };
        
        var rotation = new RotateTransform();
        rotation.CenterX = projectileImage.Width / 2;
        rotation.CenterY = projectileImage.Height / 2;
        projectileImage.RenderTransform = rotation;
        
        GameObject projectileGameObject = new GameObject(projectileImage, projectileModel);
        projectileGameObject.Rotation = rotation;
        projectileGameObject.Rotation.Angle = 90;
        
        _gameObjects.Add(projectileGameObject);
        _gameManager.AddGameObject(projectileGameObject);

        var eventData = new ObjectEventArgs(projectileGameObject, projectileImage);
        ProjectileCreated.Invoke(this, eventData);
    }
}
