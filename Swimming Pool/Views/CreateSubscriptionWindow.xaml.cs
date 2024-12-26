using Swimming_Pool.Models;
using Swimming_Pool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Swimming_Pool.Views;

public partial class CreateSubscriptionWindow : Window
{
    private CreateUpdateSubscriptionViewModel createSubscriptionViewModel = new();
    public CreateUpdateSubscriptionViewModel CreateSubscriptionViewModel { get => createSubscriptionViewModel; set => createSubscriptionViewModel = value; }

    public CreateSubscriptionWindow()
    {
        DataContext = CreateSubscriptionViewModel;
        InitializeComponent();
        Initialize();
    }

    public async void Initialize()
    {
        StartDatePicker.Value = DateTime.Now;
        EndDatePicker.Value = DateTime.Now.AddDays(30); // Default to 1 month
        CreateSubscriptionViewModel.Clients = await Database.GetAllClients();
        CreateSubscriptionViewModel.SubscriptionTypes = await Database.GetAllSubscriptionTypes();
    }

    private async void CancelCreationButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.MainWindowViewModel.Subscriptions = await Database.GetAllSubscriptions();
        Close();
    }

    private async void CreateSubscriptionButton_Click(object sender, RoutedEventArgs e)
    {
        bool isOkay = CheckAbilityToCreate();
        if (!isOkay)
        {
            MessageBox.Show("Fix all of the errors first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            CreateSubscriptionButton.IsEnabled = false;
            return;
        }

        DateTime startDate = StartDatePicker.Value ?? DateTime.Now;
        DateTime endDate = EndDatePicker.Value ?? DateTime.Now.AddDays(30);

        Client selectedClient = (Client)ClientComboBox.SelectedItem;
        SubscriptionType selectedSubscriptionType = (SubscriptionType)SubscriptionTypeComboBox.SelectedItem;

        await Database.CreateSubscription(
            selectedSubscriptionType.SubscriptionTypeId,
            float.Parse(PriceTextBox.Text),
            startDate,
            endDate,
            selectedClient.ClientId);

        MainWindow.MainWindowViewModel.Subscriptions = await Database.GetAllSubscriptions();
        Close();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => CheckAbilityToCreate();

    private bool CheckAbilityToCreate()
    {
        bool isOkay = true;

        if (SubscriptionTypeComboBox.SelectedItem == null)
        {
            isOkay = false;
        }

        if (string.IsNullOrWhiteSpace(PriceTextBox.Text) || !float.TryParse(PriceTextBox.Text, out _))
        {
            isOkay = false;
        }

        if (StartDatePicker.Value == null || StartDatePicker.Value == DateTime.MinValue)
        {
            isOkay = false;
        }

        if (EndDatePicker.Value == null || EndDatePicker.Value == DateTime.MinValue || EndDatePicker.Value <= StartDatePicker.Value)
        {
            isOkay = false;
        }

        if (ClientComboBox.SelectedItem == null)
        {
            isOkay = false;
        }

        CreateSubscriptionButton.IsEnabled = isOkay;
        return isOkay;
    }

    private void SearchButtonClient_Click(object sender, RoutedEventArgs e)
    {
        if ((bool)SearchClientToggleButton.IsChecked!)
        {
            ClientComboBox.Visibility = Visibility.Collapsed;
            ClientSearchTextBox.Visibility = Visibility.Visible;
            return;
        }
        ClientComboBox.Visibility = Visibility.Visible;
        ClientSearchTextBox.Visibility = Visibility.Collapsed;
    }

    private async void TextBoxClientSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        CreateSubscriptionViewModel.Clients = await Database.GetClientsFilteredByName(ClientSearchTextBox.Text);
    }

    private async void TextBoxSubscriptionTypeSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        CreateSubscriptionViewModel.SubscriptionTypes = await Database.GetSubscriptionTypeFilteredByName(ClientSearchTextBox.Text);
    }

    private void SearchButtonSubscriptionType_Click(object sender, RoutedEventArgs e)
    {
        if ((bool)SearchClientToggleButton.IsChecked!)
        {
            SubscriptionTypeComboBox.Visibility = Visibility.Collapsed;
            SubscriptionTypeSearchTextBox.Visibility = Visibility.Visible;
            return;
        }
        SubscriptionTypeComboBox.Visibility = Visibility.Visible;
        SubscriptionTypeSearchTextBox.Visibility = Visibility.Collapsed;
    }

    private void ClientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CheckAbilityToCreate();
    }

    private void SubscriptionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CheckAbilityToCreate();
    }
}