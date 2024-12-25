using CommunityToolkit.Mvvm.ComponentModel;
using Swimming_Pool.Models;
using System.Collections.ObjectModel;

namespace Swimming_Pool.ViewModels;

public partial class CreateUpdateSpecializationViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<SpecializationType> _specializationTypes = [];
}