using Swimming_Pool.Models;
using Swimming_Pool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Swimming_Pool.Views;

public partial class CreateTrainingWindow : Window
{
    private static CreateUpdateTrainingViewModel createUpdateTrainingViewModel = new();

    public static CreateUpdateTrainingViewModel CreateUpdateTrainingViewModel { get => createUpdateTrainingViewModel; set => createUpdateTrainingViewModel = value; }
    public static List<int> ClientIds { get; set; } = [];

    public CreateTrainingWindow()
    {
        DataContext = CreateUpdateTrainingViewModel;
        InitializeComponent();
        Initialize();
    }

    private async void Initialize()
    {
        DateTimePicker.Value = DateTime.Now;
        CreateUpdateTrainingViewModel.Clients = await Database.GetAllClients();
        CreateUpdateTrainingViewModel.Pools = await Database.GetAllPools();
        CreateUpdateTrainingViewModel.Instructors = await Database.GetAllInstructors();
        CreateUpdateTrainingViewModel.ClientsTrainings = [];
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

        Pool pool = (Pool)PoolComboBox.SelectedItem;
        Instructor instructor = (Instructor)InstructorComboBox.SelectedItem;

        await Database.CreateTraining(dateTime, TrainingTypeTextBox.Text, pool.PoolId, instructor.InstructorId, ClientIds);
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

        CreateTrainingButton.IsEnabled = isOkay;
        return isOkay;
    }

    private void AddClientButton_Click(object sender, RoutedEventArgs e)
    {
        Client client = (Client)ClientComboBox.SelectedItem;
        if (ClientIds.Contains(client.ClientId))
        {
            return;
        }
        ClientIds.Add(client.ClientId);
        CreateUpdateTrainingViewModel.ClientsTrainings.Add(new ClientTrainingEnrollment() {  ClientId = client.ClientId, ClientName = client.FirstName + " " + client.LastName });
    }
}