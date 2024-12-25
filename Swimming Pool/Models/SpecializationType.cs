using CommunityToolkit.Mvvm.Input;
using Swimming_Pool.Views;

namespace Swimming_Pool.Models;

public partial class SpecializationType
{
    public int InstructorSpecializationId { get; set; }
    public string? Specialization { get; set; }

    [RelayCommand]
    private static void UpdateSpecializationType(int specializationId)
    {
        UpdateSpecializationTypeWindow updateSpecializationTypeWindow = new()
        {
            Owner = MainWindow.MainWindowInstance
        };
        updateSpecializationTypeWindow.Initialize(specializationId);
        updateSpecializationTypeWindow.ShowDialog();
    }
}