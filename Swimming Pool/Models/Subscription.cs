using CommunityToolkit.Mvvm.Input;
using Swimming_Pool.Views;

namespace Swimming_Pool.Models;

public partial class Subscription
{
    public int SubscriptionId { get; set; }
    public string? SubscriptionType { get; set; }
    public int VisitCount { get; set; }
    public float Price { get; set; }
    public string PriceAsString { get => Price.ToString("F2") + "₴"; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ClientId { get; set; }
    public string? ClientName { get; set; }

    [RelayCommand]
    private static void UpdateSubscription(int subscriptionId)
    {
        UpdateSubscriptionWindow updateSubscriptionWindow = new()
        {
            Owner = MainWindow.MainWindowInstance
        };
        updateSubscriptionWindow.Initialize(subscriptionId);
        updateSubscriptionWindow.ShowDialog();
    }

    public async Task SetClientNameAsync() => ClientName = await Database.GetClientNameByIdAsync(ClientId);
}