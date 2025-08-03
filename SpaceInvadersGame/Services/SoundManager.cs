using System.Runtime.InteropServices;
using Windows.Media.Core;
using Windows.Media.Playback;
namespace SpaceInvadersGame;

public class SoundManager
{
    public void PlaySound(string fileName)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Assets", fileName);
        PlaySound(path, IntPtr.Zero, SoundFlags.SND_FILE | SoundFlags.SND_ASINC);
    }
    
    [Flags]
    enum SoundFlags : int
    {
        SND_ASINC = 0x0001,
        SND_FILE = 0x0002000
    }
    
    [DllImport("winmm.dll", SetLastError = true)]
    static extern bool PlaySound(string caminhoSom, IntPtr hWnd, SoundFlags lpszSoundName);
}
