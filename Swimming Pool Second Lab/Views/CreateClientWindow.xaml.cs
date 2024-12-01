using System.Windows;
using System.Windows.Controls;

namespace Swimming_Pool.Views;

public partial class CreateClientWindow : Window
{
    public CreateClientWindow() => InitializeComponent();

    private async void CancelCreationButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.MainWindowViewModel.Clients = await Database.GetAllClients();
        Close();
    }

    private async void CreateClientButton_Click(object sender, RoutedEventArgs e)
    {
        bool isOkay = CheckAbilityToCreate();
        if (!isOkay)
        {
            MessageBox.Show("Fix all of the errors first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            CreateClientButton.IsEnabled = false;
            return;
        }
        await Database.CreateClient(FirstNameTextBox.Text, LastNameTextBox.Text, int.Parse(AgeTextBox.Text), PhoneNumberTextBox.Text, EmailAddressTextBox.Text);
        MainWindow.MainWindowViewModel.Clients = await Database.GetAllClients();
        Close();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => CheckAbilityToCreate();

    private bool CheckAbilityToCreate()
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
            if (!int.TryParse(AgeTextBox.Text, out int age))
            {
                isOkay = false;
            }
        }

        CreateClientButton.IsEnabled = isOkay;
        return isOkay;
    }
}