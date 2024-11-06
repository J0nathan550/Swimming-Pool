using CommunityToolkit.Mvvm.ComponentModel;
using Swimming_Pool_Second_Lab.Models;
using System.Collections.ObjectModel;

namespace Swimming_Pool_Second_Lab.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Client> clients = [];
    [ObservableProperty]
    private ObservableCollection<Instructor> instructors = [];
    [ObservableProperty]
    private ObservableCollection<Training> trainings = [];
    [ObservableProperty]
    private Training? selectedTraining;
    [ObservableProperty]
    private int selectedClientId;
    [ObservableProperty]
    private int selectedInstructorId;
}