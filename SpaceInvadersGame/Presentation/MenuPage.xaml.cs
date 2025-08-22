using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace SpaceInvadersGame.Presentation;

public sealed partial class MenuPage : Page
{
    public MenuPage()
    {
        this.InitializeComponent();
    }

    private void StartGameButton_Click(object sender, RoutedEventArgs e)
    {
        this.Frame.Navigate(typeof(MainPage), UsernameTextbox);
    }
    
    private async void ShowScoreRankingDialog(object sender, RoutedEventArgs e)
    {
        var scoreListLines = await GetScoreAsync();
        var stringBuilder = new StringBuilder();
        
        foreach (var line in scoreListLines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            stringBuilder.Append(line);
            stringBuilder.Append(Environment.NewLine);
        }
        
        ContentDialog scoreRankingDialog = new ContentDialog
        {
            Title = "Score - Ranking",
            Content = stringBuilder.ToString(),
            SecondaryButtonText = "Resetar",
            CloseButtonText = "Fechar",
            XamlRoot = this.XamlRoot,
        };
        
        ContentDialogResult result = await scoreRankingDialog.ShowAsync();
        if (result == ContentDialogResult.Secondary)
        {
            await ClearLocalFolderAsync();
            scoreRankingDialog.Content = await GetScoreAsync();
        }
    }
    
    private async Task<IList<string>> GetScoreAsync()
    {
        try
        {
            var storageFolder = ApplicationData.Current.LocalFolder;
            var fileName = "Score.txt";
            var scoreFile = await storageFolder.GetFileAsync(fileName);
            
            var lines = await FileIO.ReadLinesAsync(scoreFile);
            return lines;
        } catch(Exception ex) {}

        return new List<string>();
    }
    
    public async Task ClearLocalFolderAsync()
    {
        var storageFolder = ApplicationData.Current.LocalFolder;
        var fileName = "Score.txt";
        var scoreFile = await storageFolder.GetFileAsync(fileName);
        await FileIO.WriteTextAsync(scoreFile, string.Empty);
    }
}

