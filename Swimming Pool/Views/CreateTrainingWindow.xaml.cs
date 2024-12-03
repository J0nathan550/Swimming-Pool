using Swimming_Pool.Models;
using Swimming_Pool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Swimming_Pool.Views;

public partial class CreateTrainingWindow : Window
{
    private CreateUpdateTrainingViewModel createTrainingViewModel = new();

    public CreateUpdateTrainingViewModel CreateTrainingViewModel { get => createTrainingViewModel; set => createTrainingViewModel = value; }

    public CreateTrainingWindow()
    {
        DataContext = CreateTrainingViewModel;
        InitializeComponent();
        Initialize();
    }

    private async void Initialize()
    {
        DateTimePicker.Value = DateTime.Now;
        CreateTrainingViewModel.Clients = await Database.GetAllClients();
        CreateTrainingViewModel.Instructors = await Database.GetAllInstructors();
    }

    private async void CancelCreationButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.MainWindowViewModel.Trainings = await Database.GetAllTrainings();
        Close();
    }

    private async void CreateTrainingButton_Click(object sender, RoutedEventArgs e)
    {
        bool isOkay = CheckAbilityToCreate();
        if (!isOkay)
        {
            MessageBox.Show("Fix all of the errors first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            CreateTrainingButton.IsEnabled = false;
            return;
        }
        DateTime dateTime = DateTime.Now;
        if (DateTimePicker.Value != null)
        {
            dateTime = (DateTime)DateTimePicker.Value;
        }

        Client client = (Client)ClientComboBox.SelectedItem;
        Instructor instructor = (Instructor)InstructorComboBox.SelectedItem;

        await Database.CreateTraining(dateTime, TrainingTypeTextBox.Text, PoolNameTextBox.Text, client.ClientId, instructor.InstructorId);
        MainWindow.MainWindowViewModel.Trainings = await Database.GetAllTrainings();
        Close();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => CheckAbilityToCreate();

    private bool CheckAbilityToCreate()
    {
        bool isOkay = true;

        if (DateTimePicker.Value == null || DateTimePicker.Value == DateTime.MinValue)
        {
            isOkay = false;
        }

        if (string.IsNullOrWhiteSpace(TrainingTypeTextBox.Text))
        {
            isOkay = false;
        }

        if (string.IsNullOrWhiteSpace(PoolNameTextBox.Text))
        {
            isOkay = false;
        }

        CreateTrainingButton.IsEnabled = isOkay;
        return isOkay;
    }
}