using CommunityToolkit.Mvvm.Input;
using Swimming_Pool_Second_Lab.Views;

namespace Swimming_Pool_Second_Lab.Models;

public partial class Instructor
{
    public int InstructorId { get; set; }
    public int Age { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string? Specialization { get; set; }

    [RelayCommand]
    private static void UpdateInstructor(int instructorId)
    {
        UpdateInstructorWindow updateInstructorWindow = new()
        {
            Owner = MainWindow.MainWindowInstance
        };
        updateInstructorWindow.Initalize(instructorId);
        updateInstructorWindow.ShowDialog();
    }
}