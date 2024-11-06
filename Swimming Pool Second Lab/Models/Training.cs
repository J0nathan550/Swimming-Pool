using CommunityToolkit.Mvvm.Input;
using Swimming_Pool_Second_Lab.Views;

namespace Swimming_Pool_Second_Lab.Models;

public partial class Training
{
    public int TrainingId { get; set; }
    public DateTime Date { get; set; }
    public string? TrainingType { get; set; }
    public string? PoolName { get; set; }
    public int ClientId { get; set; }
    public string? ClientName { get; set; }
    public int InstructorId { get; set; }
    public string? InstructorName { get; set; }

    [RelayCommand]
    private static void UpdateTraining(int trainingId)
    {
        UpdateTrainingWindow updateTrainingWindow = new()
        {
            Owner = MainWindow.MainWindowInstance
        };
        updateTrainingWindow.Initalize(trainingId);
        updateTrainingWindow.ShowDialog();
    }
}