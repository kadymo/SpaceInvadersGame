using Microsoft.UI.Xaml.Media.Imaging;
using SpaceInvadersGame.Models;

namespace SpaceInvadersGame;

public sealed partial class MainPage : Page
{

    private readonly GameManager _gameManager;
    private readonly InputManager _inputManager;
    
    private readonly List<GameObject> _gameObjects = new List<GameObject>();
    
    private DateTime _lastFrameTime;
    
    private Image playerImage;
    public MainPage()
    {
        this.InitializeComponent();
        _gameManager = new GameManager(GameCanvas);
        _inputManager = new InputManager(GameCanvas);
        
        this.Loaded += OnPageLoaded;
        this.Unloaded += OnPageUnloaded;
    }
    
    private void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        // Start loop when the page is ready
        
        _gameManager.Start();
        _lastFrameTime = DateTime.Now;
        CompositionTarget.Rendering += OnRendering;

        playerImage = new Image
        {
            Source = new BitmapImage(new Uri("ms-appx:///Assets/Corinthians_simbolo.png")),
            Width = 50,
            Height = 50,
        };
        
        Canvas.SetZIndex(playerImage, 10);
        GameCanvas.Children.Add(playerImage);
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
