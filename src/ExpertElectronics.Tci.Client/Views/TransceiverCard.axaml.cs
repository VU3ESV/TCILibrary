using Avalonia.Controls;
using Avalonia.Interactivity;
using ExpertElectronics.Tci.Client.ViewModels;

namespace ExpertElectronics.Tci.Client.Views;

public partial class TransceiverCard : UserControl
{
    public TransceiverCard()
    {
        InitializeComponent();
    }

    private async void OnStartRxClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is TransceiverViewModel vm) await vm.StartRxAudioAsync();
    }

    private async void OnStopRxClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is TransceiverViewModel vm) await vm.StopRxAudioAsync();
    }

    private async void OnStartTxClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is TransceiverViewModel vm) await vm.StartTxAudioAsync();
    }

    private async void OnStopTxClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is TransceiverViewModel vm) await vm.StopTxAudioAsync();
    }
}
