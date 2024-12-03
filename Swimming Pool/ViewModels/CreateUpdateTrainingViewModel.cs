using CommunityToolkit.Mvvm.ComponentModel;
using Swimming_Pool.Models;
using System.Collections.ObjectModel;

namespace Swimming_Pool.ViewModels;

public partial class CreateUpdateTrainingViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ClientTrainingEnrollment> _clientsTrainings = [];
    [ObservableProperty]
    private ObservableCollection<Instructor> _instructors = [];
    [ObservableProperty]
    private ObservableCollection<Client> _clients = [];
    [ObservableProperty]
    private ObservableCollection<Pool> _pools = [];
}