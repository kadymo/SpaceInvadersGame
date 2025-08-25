using Windows.Foundation;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using SpaceInvadersGame.Models;
using SpaceInvadersGame.Presentation;
using SpaceInvadersGame.ViewModels;

namespace SpaceInvadersGame;

public sealed partial class MainPage : Page
{
    private DateTime _lastFrameTime;
    private StackPanel _livesPanel;
    
    public MainPage()
    {
        this.InitializeComponent();
        ViewModel = new MainViewModel();
        this.DataContext = ViewModel;
        
        CreateLivesPanel();
        
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
        ViewModel.LifesChanged += OnLifesChanged;
        ViewModel.GameOver += OnGameOver;
        ViewModel.GameRestarted += OnGameRestarted;
    }
    
    private void CreateLivesPanel()
    {
        _livesPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 5,
            Margin = new Thickness(20, 20, 0, 0)
        };
        
        HeaderGrid.Children.Remove(Lifes); 
        Grid.SetColumn(_livesPanel, 2); 
        _livesPanel.Margin = new Thickness(40, 20, 0, 0); 
        HeaderGrid.Children.Add(_livesPanel);
    }
    
    private void UpdateLifesDisplay(int lifes)
    {
        _livesPanel.Children.Clear();
        
        for (int i = 0; i < lifes; i++)
        {
            Image lifeImage = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/Life.png")), // ou Life.png, PlayerShip.png, etc.
                Width = 30,
                Height = 30,
            };
            _livesPanel.Children.Add(lifeImage);
        }
    }
    
    // Page Event Handlers
    
    private void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        this.Focus(FocusState.Programmatic);
        this.KeyDown += OnPageKeyDown;
        this.KeyUp += OnPageKeyUp;

        Score.Text = "SCORE: 0";
        Wave.Text = "WAVE: 1";
        UpdateLifesDisplay(3);
        
        ViewModel.Start();
        
        _lastFrameTime = DateTime.Now;
        
        CompositionTarget.Rendering += OnRendering;
    }
    
    private void OnPageUnloaded(object sender, RoutedEventArgs e)
    {
        CompositionTarget.Rendering -= OnRendering;
        ViewModel.Stop();
    }

    private void InitializeGameUI()
    {
        Score.Text = "SCORE: 0";
        Wave.Text = "WAVE: 1";
        UpdateLifesDisplay(3);
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

    private void OnLifesChanged(object? sender, int value)
    {
        UpdateLifesDisplay(value);
    }
    
    private void OnGameRestarted(object? sender, EventArgs e)
    {
        GameCanvas.Children.Clear();
        InitializeGameUI();
        this.Focus(FocusState.Programmatic);
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
    private async void OnGameOver(object sender, GameOverEventArgs e)
    {
        ShowGameOverDialog();
    }

    // private async void ShowGameOverDialog()
    // {
    //     ContentDialog gameOverDialog = new ContentDialog
    //     {
    //         Title = "Game Over",
    //         Content = Score.Text,
    //         PrimaryButtonText = "Menu",
    //         SecondaryButtonText = "Salvar pontuação",
    //         XamlRoot = this.XamlRoot,
    //     };
    //     
    //     ContentDialogResult result = await gameOverDialog.ShowAsync();
    //     if (result == ContentDialogResult.Primary)
    //     {
    //         ViewModel.Stop();
    //         this.Frame.Navigate(typeof(MenuPage));
    //     }  else if (result == ContentDialogResult.Secondary)
    //     {
    //         await ViewModel.SaveScoreAsync();
    //         ViewModel.Stop();
    //         this.Frame.Navigate(typeof(MenuPage));
    //     } 
    // }
    
    private async void ShowGameOverDialog()
    {
        ContentDialog gameOverDialog = new ContentDialog
        {
            Title = "Game Over",
            Content = Score.Text,
            PrimaryButtonText = "Jogar Novamente",
            SecondaryButtonText = "Menu",
            CloseButtonText = "Salvar Pontuação",
            XamlRoot = this.XamlRoot,
        };
        
        ContentDialogResult result = await gameOverDialog.ShowAsync();
        
        if (result == ContentDialogResult.Primary)
        {
            // Jogar Novamente
            ViewModel.RestartGame();
        }
        else if (result == ContentDialogResult.Secondary)
        {
            // Menu
            ViewModel.Stop();
            this.Frame.Navigate(typeof(MenuPage));
        }
        else if (result == ContentDialogResult.None)
        {
            // Salvar Pontuação (CloseButton)
            await ViewModel.SaveScoreAsync();
            ViewModel.Stop();
            this.Frame.Navigate(typeof(MenuPage));
        }
    }
}
