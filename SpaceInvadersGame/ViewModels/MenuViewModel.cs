using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SpaceInvadersGame.ViewModels;

public partial class MenuViewModel : ObservableObject
{
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(IsStartButtonEnabled))]
    private string _username;
    
    public bool IsStartButtonEnabled => !string.IsNullOrWhiteSpace(_username);
}
