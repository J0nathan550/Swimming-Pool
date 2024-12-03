using CommunityToolkit.Mvvm.ComponentModel;
using Swimming_Pool.Models;
using System.Collections.ObjectModel;

namespace Swimming_Pool.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Client> clients = [];
    [ObservableProperty]
    private ObservableCollection<Client> clientsWithNull = [];
    [ObservableProperty]
    private ObservableCollection<Instructor> instructors = [];
    [ObservableProperty]
    private ObservableCollection<Instructor> instructorsWithNull = [];
    [ObservableProperty]
    private ObservableCollection<Training> trainings = [];
    [ObservableProperty]
    private Training? selectedTraining;
    [ObservableProperty]
    private int selectedClientId;
    [ObservableProperty]
    private int selectedInstructorId;
}