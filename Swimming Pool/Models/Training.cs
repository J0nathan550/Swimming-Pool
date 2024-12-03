using CommunityToolkit.Mvvm.Input;
using Swimming_Pool.Views;

namespace Swimming_Pool.Models;

public partial class Training
{
    public int TrainingId { get; set; }
    public DateTime Date { get; set; }
    public string? TrainingType { get; set; }
    public int PoolId { get; set; }
    public string? PoolName { get; set; }
    public string? ClientNames { get; set; }
    public int InstructorId { get; set; }
    public string? InstructorName { get; set; }

    [RelayCommand]
    private static void UpdateTraining(int trainingId)
    {
        UpdateTrainingWindow updateTrainingWindow = new()
        {
            Owner = MainWindow.MainWindowInstance
        };
        updateTrainingWindow.Initialize(trainingId);
        updateTrainingWindow.ShowDialog();
    }
}