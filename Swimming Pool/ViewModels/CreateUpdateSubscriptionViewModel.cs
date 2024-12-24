using CommunityToolkit.Mvvm.ComponentModel;
using Swimming_Pool.Models;
using System.Collections.ObjectModel;

namespace Swimming_Pool.ViewModels;

public partial class CreateUpdateSubscriptionViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Client> _clients = [];
    [ObservableProperty]
    private ObservableCollection<SubscriptionType> _subscriptionTypes = [];
}