using CommunityToolkit.Mvvm.ComponentModel;
using Swimming_Pool.Models;
using System.Collections.ObjectModel;

namespace Swimming_Pool.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Client> clients = [];
    [ObservableProperty]
    private ObservableCollection<Instructor> instructors = [];
    [ObservableProperty]
    private ObservableCollection<Training> trainings = [];
    [ObservableProperty]
    private ObservableCollection<Pool> pools = [];
    [ObservableProperty]
    private ObservableCollection<Subscription> subscriptions = [];
    [ObservableProperty]
    private ObservableCollection<SubscriptionType> subscriptionTypes = [];
    [ObservableProperty]
    private ObservableCollection<SpecializationType> specializationTypes = [];
}