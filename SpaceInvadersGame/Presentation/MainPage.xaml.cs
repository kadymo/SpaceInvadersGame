using Windows.Foundation;
using Microsoft.UI.Xaml.Input;
using SpaceInvadersGame.ViewModels;

namespace SpaceInvadersGame;

public sealed partial class MainPage : Page
{
    private DateTime _lastFrameTime;
    
    public MainPage()
    {
        this.InitializeComponent();
        ViewModel = new MainViewModel();
        this.DataContext = ViewModel;
        
        this.Loaded += OnPageLoaded;
        this.Unloaded += OnPageUnloaded;

        ViewModel.PlayerCreated += OnPlayerCreated;
        ViewModel.EnemyCreated += OnEnemyCreated;
        ViewModel.EnemyRemoved += OnEnemyRemoved;
        ViewModel.ProjectileCreated += OnProjectileCreated;
        ViewModel.ProjectileRemoved += OnProjectileRemoved;
        ViewModel.ObstacleCreated += OnObstacleCreated;
        ViewModel.ObstacleRemoved += OnObstacleRemoved;
        ViewModel.ExplosionEffectCreated += OnExplosionEffectCreated;
        ViewModel.ExplosionEffectRemoved += OnExplosionEffectRemoved;
    }
    
    // Page Event Handlers
    
    private void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        this.Focus(FocusState.Programmatic);
        this.KeyDown += OnPageKeyDown;
        this.KeyUp += OnPageKeyUp;

        Score.Text = "SCORE: 0";
        
        ViewModel.Start();
        
        _lastFrameTime = DateTime.Now;
        
        CompositionTarget.Rendering += OnRendering;
    }
    
    private void OnPageUnloaded(object sender, RoutedEventArgs e)
    {
        CompositionTarget.Rendering -= OnRendering;
        ViewModel.Stop();
    }
    
    // Navigation Handlers

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is TextBox parameter)
        {
            Username.Text = parameter.Text;
        }
    }

    private void OnRendering(object? sender, object e)
    {
        var currentTime = DateTime.Now;
        var deltaTime = currentTime - _lastFrameTime;
        _lastFrameTime = currentTime;
        
        var bounds = new Rect(0, 0, (float)GameCanvas.ActualWidth, (float)GameCanvas.ActualHeight);
        ViewModel.Update(deltaTime, bounds);
    }
    
    // Keyboard Event Handlers
    
    private void OnPageKeyDown(object sender, KeyRoutedEventArgs e) => ViewModel.InputManager.KeyDown(e.Key);
    private void OnPageKeyUp(object sender, KeyRoutedEventArgs e) => ViewModel.InputManager.KeyUp(e.Key);

    
    private void OnPlayerCreated(object  sender, ObjectEventArgs data)
    {
        // Canvas.SetZIndex(data.GameObject.View, 10);
        GameCanvas.Children.Add(data.ImageElement);
    }

    private void OnEnemyCreated(object  sender, ObjectEventArgs data)
    {
        GameCanvas.Children.Add(data.ImageElement);
    }
    
    private void OnEnemyRemoved(object  sender, ObjectEventArgs data)
    { 
        GameCanvas.Children.Remove(data.GameObject.View);
    }

    private void OnProjectileCreated(object  sender, ObjectEventArgs data)
    {
        // Canvas.SetZIndex(data.GameObject.View, 10);
        GameCanvas.Children.Add(data.ImageElement);
    }

    private void OnProjectileRemoved(object sender, ObjectEventArgs data)
    {
        GameCanvas.Children.Remove(data.GameObject.View);
    }
    
    private void OnObstacleCreated(object sender, ObjectEventArgs data)
    {
        GameCanvas.Children.Add(data.ImageElement);
    }
    
    private void OnObstacleRemoved(object sender, ObjectEventArgs data)
    {
        GameCanvas.Children.Remove(data.GameObject.View);
    }

    private void OnExplosionEffectCreated(object sender, ObjectEventArgs data)
    {
        GameCanvas.Children.Add(data.ImageElement);
    }
    
    private void OnExplosionEffectRemoved(object sender, ObjectEventArgs data)
    {
        GameCanvas.Children.Remove(data.ImageElement);
    }
}
