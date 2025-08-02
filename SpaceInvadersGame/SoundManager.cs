using Windows.Media.Core;
using Windows.Media.Playback;
namespace SpaceInvadersGame;

public class SoundManager
{
    private List<MediaPlayerElement> _mediaPlayers = new List<MediaPlayerElement>();
    private int _currentPlayerIndex = 0;
    
    private readonly Uri _projectileSoundUri = new Uri("ms-appx:///Assets/ProjectileSound.mp4");

    public SoundManager(MediaPlayerElement[] players)
    {
        _mediaPlayers = new List<MediaPlayerElement>(players);
        foreach (var playerElement in players)
        {
            playerElement.SetMediaPlayer(new MediaPlayer());
        }
    }
    
    public void PlaySound(Uri soundUri)
    {
        var playerElement = _mediaPlayers[_currentPlayerIndex];
        var mediaPlayer = playerElement.MediaPlayer;
        
        mediaPlayer.Source = MediaSource.CreateFromUri(soundUri);
        mediaPlayer.Play();
        _currentPlayerIndex = (_currentPlayerIndex + 1) % _mediaPlayers.Count;
    }
}
