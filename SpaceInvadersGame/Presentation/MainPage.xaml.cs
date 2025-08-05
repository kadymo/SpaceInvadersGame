
using Windows.Media.Core;
using Windows.Media.Playback;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using SpaceInvadersGame.Models;

namespace SpaceInvadersGame;

public sealed partial class MainPage : Page
{
    private readonly GameManager _gameManager;
    private readonly InputManager _inputManager;
    private readonly SoundManager _soundManager;
    
    private readonly List<GameObject> _gameObjects = new List<GameObject>();
    
    private DateTime _lastFrameTime;
    
    public MainPage()
    {
        this.InitializeComponent();
        
        _inputManager = new InputManager();
        _soundManager = new SoundManager();
        
        _gameManager = new GameManager(_gameObjects, _inputManager, _soundManager);
        
        this.Loaded += OnPageLoaded;
        this.Unloaded += OnPageUnloaded;
    }
    
    private void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        // Start loop when the page is ready
        this.Focus(FocusState.Programmatic);

        Score.Text = "SCORE: ";
        _gameManager.Start();
        _gameManager.OnProjectileFired += (object sender, Projectile projectileModel) =>
        { 
            CreateProjectileView(sender, projectileModel);
        };
        _gameManager.OnProjectileHit += async (object sender, CollisionEventArgs collisionData) =>
        {
            Score.Text = "SCORE: " + _gameManager.Score;
            
            var enemyGameObject = collisionData.EnemyGameObject;
            var projectileGameObject = collisionData.ProjectileGameObject;

            var explosionLeft = Canvas.GetLeft(enemyGameObject.View);
            var explosionTop = Canvas.GetTop(enemyGameObject.View);
            
            _gameObjects.Remove(enemyGameObject);
            _gameObjects.Remove(projectileGameObject);
            
            GameCanvas.Children.Remove(enemyGameObject.View);
            GameCanvas.Children.Remove(projectileGameObject.View);
           
            // Explosion image renderization
            Image explosionImage = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/Explosion.gif")),
                Width = 45,
                Height = 45,
            };

            Canvas.SetLeft(explosionImage, explosionLeft);
            Canvas.SetTop(explosionImage, explosionTop);
    
            GameCanvas.Children.Add(explosionImage);
            await Task.Delay(500);
            GameCanvas.Children.Remove(explosionImage);
        };
        
        _gameManager.OnProjectileExceededScreen += (object sender, GameObject gameObject) =>
        {
            _gameObjects.Remove(gameObject);
            GameCanvas.Children.Remove(gameObject.View);
        };
        
        _lastFrameTime = DateTime.Now;
        
        this.KeyDown += OnPageKeyDown;
        this.KeyUp += OnPageKeyUp;

        Image playerImage = new Image
        {
            Source = new BitmapImage(new Uri("ms-appx:///Assets/Corinthians_simbolo.png")),
            Width = 50,
            Height = 50,
        };

        GameObject player = new GameObject(playerImage, new Player());
        _gameObjects.Add(player);
        _gameManager.AddGameObject(player);
        
        Canvas.SetZIndex(player.View, 10);
        GameCanvas.Children.Add(playerImage);
        
        CreateEnemiesGrid();
        
        CompositionTarget.Rendering += OnRendering;
    }

    private void OnPageKeyDown(object sender, KeyRoutedEventArgs e)
    {
        _inputManager.KeyDown(e.Key);
    }
    
    private void OnPageKeyUp(object sender, KeyRoutedEventArgs e)
    {
        _inputManager.KeyUp(e.Key);
    }
    
    private void OnPageUnloaded(object sender, RoutedEventArgs e)
    {
        // Stop loop when the page is unloaded
        CompositionTarget.Rendering -= OnRendering;
        _gameManager.Stop();
    }

    private void OnRendering(object sender, object e)
    {
        var currentTime = DateTime.Now;
        var deltaTime = currentTime - _lastFrameTime;
        _lastFrameTime = currentTime;
        
        _gameManager.Update(deltaTime);

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
                Canvas.SetTop(go.View, enemy.PositionY);;
            }
            
            if (go.Model is Projectile projectile)
            {
                Canvas.SetLeft(go.View, projectile.PositionX);
                Canvas.SetTop(go.View, projectile.PositionY);;
            }
        }
    }
    
    private void CreatePlayerView()
    {
        Image playerImage = new Image
        {
            Source = new BitmapImage(new Uri("ms-appx:///Assets/Corinthians_simbolo.png")),
            Width = 50,
            Height = 50,
        };

        GameObject player = new GameObject(playerImage, new Player());
        _gameObjects.Add(player);
        _gameManager.AddGameObject(player);
        
        Canvas.SetZIndex(player.View, 10);
        GameCanvas.Children.Add(playerImage);
    }

    private void CreateEnemiesView()
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
                    Source = new BitmapImage(new Uri("ms-appx:///Assets/Palmeiras_logo.svg.png")),
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
                
                Canvas.SetZIndex(enemyGameObject.View, 10);
                GameCanvas.Children.Add(enemyImage);
            }
        }
    }

    private void CreateProjectileView(object sender, Projectile projectileModel)
    {
        Image projectileImage = new Image
        {
            Source = new BitmapImage(new Uri("ms-appx:///Assets/Projectile.gif")),
            Width = 50,
            Height = 50,
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
        
        Canvas.SetZIndex(projectileGameObject.View, 10);
        GameCanvas.Children.Add(projectileImage);
        
    }
}
