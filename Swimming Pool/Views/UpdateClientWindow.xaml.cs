using Swimming_Pool.Models;
using System.Windows;
using System.Windows.Controls;

namespace Swimming_Pool.Views;

public partial class UpdateClientWindow : Window
{
    private Client? _client;
    private int _clientID = -1;

    public UpdateClientWindow() => InitializeComponent();

    public async void Initialize(int clientID)
    {
        _clientID = clientID;
        _client = await Database.GetClientById(clientID);
        FirstNameTextBox.Text = _client!.FirstName;
        LastNameTextBox.Text = _client.LastName;
        AgeTextBox.Value = _client.Age;
        EmailAddressTextBox.Text = _client.EmailAddress;
        PhoneNumberTextBox.Text = _client.PhoneNumber;
    }

    private async void CancelUpdatingButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.MainWindowViewModel.Clients = await Database.GetAllClients();
        Close();
    }

    private async void UpdateClientButton_Click(object sender, RoutedEventArgs e)
    {
        bool isOkay = CheckAbilityToUpdate();
        if (!isOkay)
        {
            MessageBox.Show("Fix all of the errors first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            UpdateClientButton.IsEnabled = false;
            return;
        }
        await Database.UpdateClient(_clientID, FirstNameTextBox.Text, LastNameTextBox.Text, int.Parse(AgeTextBox.Text), PhoneNumberTextBox.Text, EmailAddressTextBox.Text);
        MainWindow.MainWindowViewModel.Clients = await Database.GetAllClients();
        Close();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => CheckAbilityToUpdate();

    private bool CheckAbilityToUpdate()
    {
        bool isOkay = true;

        if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
        {
            isOkay = false;
        }

        if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
        {
            isOkay = false;
        }

        if (string.IsNullOrWhiteSpace(EmailAddressTextBox.Text))
        {
            isOkay = false;
        }

        if (string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text))
        {
            isOkay = false;
        }

        if (string.IsNullOrWhiteSpace(AgeTextBox.Text))
        {
            isOkay = false;
        }
        else
        {
            if (!int.TryParse(AgeTextBox.Text, out _))
            {
                isOkay = false;
            }
        }

        UpdateClientButton.IsEnabled = isOkay;
        return isOkay;
    }

    private async void DeleteClientButton_Click(object sender, RoutedEventArgs e)
    {
        if (_client == null) return;
        MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete client: {_client.FirstName} {_client.LastName}?", "Deleting Client", MessageBoxButton.YesNo, MessageBoxImage.Error);
        if (result == MessageBoxResult.Yes)
        {
            await Database.DeleteClient(_clientID);
            MainWindow.MainWindowViewModel.Clients = await Database.GetAllClients();
            Close();
        }
    }
}