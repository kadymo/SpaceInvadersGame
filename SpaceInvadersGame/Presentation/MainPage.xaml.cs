using Windows.Foundation;
using Microsoft.UI.Xaml.Input;
using SpaceInvadersGame.Models;
using SpaceInvadersGame.Presentation;
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
        ViewModel.PlayerRemoved += OnPlayerRemoved;
        ViewModel.EnemyCreated += OnEnemyCreated;
        ViewModel.EnemyRemoved += OnEnemyRemoved;
        ViewModel.ProjectileCreated += OnProjectileCreated;
        ViewModel.ProjectileRemoved += OnProjectileRemoved;
        ViewModel.ObstacleCreated += OnObstacleCreated;
        ViewModel.ObstacleRemoved += OnObstacleRemoved;
        ViewModel.ExplosionEffectCreated += OnExplosionEffectCreated;
        ViewModel.ExplosionEffectRemoved += OnExplosionEffectRemoved;
        ViewModel.GameOver += OnGameOver;
    }
    
    // Page Event Handlers
    
    private void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        this.Focus(FocusState.Programmatic);
        this.KeyDown += OnPageKeyDown;
        this.KeyUp += OnPageKeyUp;

        Score.Text = "SCORE: 0";
        Wave.Text = "WAVE: 1";
        Lifes.Text = "LIFES: 3";
        
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
    
    private void OnPlayerCreated(object  sender, PlayerGameObject player)
    {
        GameCanvas.Children.Add(player.View);
    }
    
    private void OnPlayerRemoved(object  sender, PlayerGameObject player)
    { 
        GameCanvas.Children.Remove(player.View);
    }

    private void OnEnemyCreated(object  sender, EnemyGameObject enemy)
    {
        GameCanvas.Children.Add(enemy.View);
    }
    
    private void OnEnemyRemoved(object  sender, EnemyGameObject enemy)
    { 
        GameCanvas.Children.Remove(enemy.View);
    }

    private void OnProjectileCreated(object  sender, ProjectileGameObject projectile)
    {
        GameCanvas.Children.Add(projectile.View);
    }

    private void OnProjectileRemoved(object sender, ProjectileGameObject projectile)
    {
        GameCanvas.Children.Remove(projectile.View);
    }
    
    private void OnObstacleCreated(object sender, ObstacleGameObject obstacle)
    {
        GameCanvas.Children.Add(obstacle.View);
    }
    
    private void OnObstacleRemoved(object sender, ObstacleGameObject obstacle)
    {
        GameCanvas.Children.Remove(obstacle.View);
    }

    private void OnExplosionEffectCreated(object sender, FrameworkElement element)
    {
        GameCanvas.Children.Add(element);
    }
    
    private void OnExplosionEffectRemoved(object sender, FrameworkElement element)
    {
        GameCanvas.Children.Remove(element);
    }
    
    // Modal
    private void OnGameOver(object sender, GameOverEventArgs e)
    {
        ShowGameOverDialog(e.Score);
    }

    private async void ShowGameOverDialog(int Score)
    {
        ContentDialog gameOverDialog = new ContentDialog
        {
            Title = "Game Over",
            Content = "Score: " + Score,
            PrimaryButtonText = "Menu",
            SecondaryButtonText = "Salvar pontuação",
            XamlRoot = this.XamlRoot,
        };
        
        ContentDialogResult result = await gameOverDialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            ViewModel.Stop();
            this.Frame.Navigate(typeof(MenuPage));
        }  else if (result == ContentDialogResult.Secondary)
        {
            await SaveScoreAsync();
            ViewModel.Stop();
            this.Frame.Navigate(typeof(MenuPage));
        } 
    }

    private async Task SaveScoreAsync()
    {
        try
        {
            var storageFolder = ApplicationData.Current.LocalFolder;
            var fileName = "Score.txt";
            var scoreFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            var scoreLine = $"{Score} {Username}";
            await FileIO.AppendTextAsync(scoreFile, scoreLine);
        } catch(Exception ex) {}
    }
}
