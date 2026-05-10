using Avalonia.Controls;
using Avalonia.Interactivity;
using ExpertElectronics.Tci.Client.ViewModels;

namespace ExpertElectronics.Tci.Client.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;
        Closed += (_, _) => _vm.Dispose();
    }

    private async void OnConnectClick(object? sender, RoutedEventArgs e)
    {
        await _vm.ConnectAsync();
    }

    private async void OnDisconnectClick(object? sender, RoutedEventArgs e)
    {
        await _vm.DisconnectAsync();
    }
}
