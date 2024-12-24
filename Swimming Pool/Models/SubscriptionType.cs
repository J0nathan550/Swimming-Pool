using CommunityToolkit.Mvvm.Input;
using Swimming_Pool.Views;

namespace Swimming_Pool.Models;

public partial class SubscriptionType
{
    public int SubscriptionTypeId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    [RelayCommand]
    private static void UpdateSubscriptionType(int subscriptionTypeId)
    {
        UpdateSubscriptionTypeWindow updateSubscriptionTypeWindow = new()
        {
            Owner = MainWindow.MainWindowInstance
        };
        updateSubscriptionTypeWindow.Initialize(subscriptionTypeId);
        updateSubscriptionTypeWindow.ShowDialog();
    }
}