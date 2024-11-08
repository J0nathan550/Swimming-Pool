﻿using CommunityToolkit.Mvvm.ComponentModel;
using Swimming_Pool_Second_Lab.Models;
using System.Collections.ObjectModel;

namespace Swimming_Pool_Second_Lab.ViewModels;

public partial class CreateUpdateTrainingViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Client> _clients = [];
    [ObservableProperty]
    private ObservableCollection<Instructor> _instructors = [];
}