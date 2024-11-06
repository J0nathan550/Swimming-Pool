using Swimming_Pool_Second_Lab.Models;
using Swimming_Pool_Second_Lab.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Swimming_Pool_Second_Lab.Views;

public partial class UpdateTrainingWindow : Window
{
    private CreateUpdateTrainingViewModel createTrainingViewModel = new();
    public CreateUpdateTrainingViewModel CreateTrainingViewModel { get => createTrainingViewModel; set => createTrainingViewModel = value; }

    private Training? _training;
    private int _trainingID = -1;

    public UpdateTrainingWindow()
    {
        DataContext = CreateTrainingViewModel;
        InitializeComponent();
    }

    public async void Initalize(int trainingID)
    {
        DateTimePicker.Value = DateTime.Now;
        CreateTrainingViewModel.Clients = await Database.GetAllClients();
        CreateTrainingViewModel.Instructors = await Database.GetAllInstructors();
        _trainingID = trainingID;
        _training = await Database.GetTrainingById(trainingID);
        PoolNameTextBox.Text = _training!.PoolName;
        TrainingTypeTextBox.Text = _training.TrainingType;
        Client? client = await Database.GetClientById(_training.ClientId);
        Instructor? instructor = await Database.GetInstructorById(_training.InstructorId);
        SelectItemById(ClientComboBox, client, c => c!.ClientId);
        SelectItemById(InstructorComboBox, instructor, i => i!.InstructorId);
    }

    public static void SelectItemById<T>(ComboBox comboBox, T targetItem, Func<T, int> idSelector)
    {
        if (comboBox.ItemsSource == null || targetItem == null) return;
        int targetId = idSelector(targetItem);
        int index = comboBox.ItemsSource.Cast<T>().ToList().FindIndex(item => idSelector(item) == targetId);
        comboBox.SelectedIndex = index >= 0 ? index : -1;
    }

    private async void CancelUpdatingButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.MainWindowViewModel.Trainings = await Database.GetAllTrainings();
        Close();
    }

    private async void UpdateTrainingButton_Click(object sender, RoutedEventArgs e)
    {
        bool isOkay = CheckAbilityToUpdate();
        if (!isOkay)
        {
            MessageBox.Show("Fix all of the errors first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            UpdateTrainingButton.IsEnabled = false;
            return;
        }
        DateTime dateTime = DateTime.Now;
        if (DateTimePicker.Value != null)
        {
            dateTime = (DateTime)DateTimePicker.Value;
        }
        Client client = (Client)ClientComboBox.SelectedItem;
        Instructor instructor = (Instructor)InstructorComboBox.SelectedItem;
        await Database.UpdateTraining(_trainingID, dateTime, TrainingTypeTextBox.Text, PoolNameTextBox.Text, client.ClientId, instructor.InstructorId);
        MainWindow.MainWindowViewModel.Trainings = await Database.GetAllTrainings();
        Close();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => CheckAbilityToUpdate();

    private bool CheckAbilityToUpdate()
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

        UpdateTrainingButton.IsEnabled = isOkay;
        return isOkay;
    }

    private async void DeleteTrainingButton_Click(object sender, RoutedEventArgs e)
    {
        if (_training == null) return;
        MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete training: {_training.PoolName} {_training.TrainingType}?", "Deleting Training", MessageBoxButton.YesNo, MessageBoxImage.Error);
        if (result == MessageBoxResult.Yes)
        {
            await Database.DeleteTraining(_trainingID);
            MainWindow.MainWindowViewModel.Trainings = await Database.GetAllTrainings();
            Close();
        }
    }
}