using CommunityToolkit.Mvvm.Input;
using Swimming_Pool.Views;

namespace Swimming_Pool.Models;

public partial class Client
{
    public int ClientId { get; set; }
    public int Age { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmailAddress { get; set; }

    [RelayCommand]
    private static void UpdateClient(int clientId)
    {
        UpdateClientWindow updateClientWindow = new()
        {
            Owner = MainWindow.MainWindowInstance
        };
        updateClientWindow.Initalize(clientId);
        updateClientWindow.ShowDialog();
    }
}