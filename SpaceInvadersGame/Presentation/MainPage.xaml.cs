using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using SpaceInvadersGame.Models;

namespace SpaceInvadersGame;

public sealed partial class MainPage : Page
{
    private readonly GameManager _gameManager;
    private readonly InputManager _inputManager;
    
    private readonly List<GameObject> _gameObjects = new List<GameObject>();
    
    private DateTime _lastFrameTime;
    
    public MainPage()
    {
        this.InitializeComponent();
        _inputManager = new InputManager();
        _gameManager = new GameManager(_inputManager);
        
        this.Loaded += OnPageLoaded;
        this.Unloaded += OnPageUnloaded;
    }
    
    private void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        // Start loop when the page is ready
        this.Focus(FocusState.Programmatic);
        
        _gameManager.Start();
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
        }
    }
}
