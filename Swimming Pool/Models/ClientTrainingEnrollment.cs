using CommunityToolkit.Mvvm.Input;
using Swimming_Pool.Views;
using System.Windows;

namespace Swimming_Pool.Models;

public partial class ClientTrainingEnrollment
{
    public int ClientId { get; set; }
    public string? ClientName { get; set; }
    public int TrainingId { get; set; }

    [RelayCommand]
    private async Task RemoveEnrollment(int clientId)
    {
        Client? client = await Database.GetClientById(clientId);
        if (client == null)
        {
            MessageBox.Show("This client doesn't exist anymore.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateTrainingWindow.CreateTrainingViewModel.ClientsTrainings = await Database.GetAllEnrollments(TrainingId);
            return;
        }
        MessageBoxResult result = MessageBox.Show($"Client - {client.FirstName} {client.LastName} will be removed.\nAre you sure?", "Removing Client", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            await Database.DeleteTrainingEnrollmentAsync(TrainingId, clientId);
            UpdateTrainingWindow.CreateTrainingViewModel.ClientsTrainings = await Database.GetAllEnrollments(TrainingId);
            return;
        }
    }

    [RelayCommand]
    private void RemoveEnrollmentFromList(int clientId)
    {
        CreateTrainingWindow.ClientIds.Remove(clientId);
        CreateTrainingWindow.CreateUpdateTrainingViewModel.ClientsTrainings.Remove(this);
    }
}