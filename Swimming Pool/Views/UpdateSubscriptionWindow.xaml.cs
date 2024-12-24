using Swimming_Pool.Models;
using Swimming_Pool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Swimming_Pool.Views;

public partial class UpdateSubscriptionWindow : Window
{
    private CreateUpdateSubscriptionViewModel createSubscriptionViewModel = new();
    public CreateUpdateSubscriptionViewModel CreateSubscriptionViewModel { get => createSubscriptionViewModel; set => createSubscriptionViewModel = value; }

    private Subscription? _subscription;
    private int _subscriptionID = -1;

    public UpdateSubscriptionWindow()
    {
        DataContext = CreateSubscriptionViewModel;
        InitializeComponent();
    }

    public async void Initialize(int subscriptionID)
    {
        StartDatePicker.Value = DateTime.Now;
        EndDatePicker.Value = DateTime.Now.AddDays(30);
        CreateSubscriptionViewModel.Clients = await Database.GetAllClients();
        CreateSubscriptionViewModel.SubscriptionTypes = await Database.GetAllSubscriptionTypes();
        _subscriptionID = subscriptionID;
        _subscription = await Database.GetSubscriptionById(subscriptionID);

        // Populate fields with existing subscription data
        PriceTextBox.Text = _subscription.Price.ToString("F2");
        StartDatePicker.Value = _subscription.StartDate;
        EndDatePicker.Value = _subscription.EndDate;

        Client? client = await Database.GetClientById(_subscription.ClientId);
        SelectItemById(ClientComboBox, client, c => c!.ClientId);
        SubscriptionType? subscriptionType = await Database.GetSubscriptionTypeById(_subscription.SubscriptionTypeId);
        SelectItemById(SubscriptionTypeComboBox, subscriptionType, s => s!.SubscriptionTypeId);
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
        MainWindow.MainWindowViewModel.Subscriptions = await Database.GetAllSubscriptions();
        Close();
    }

    private async void UpdateSubscriptionButton_Click(object sender, RoutedEventArgs e)
    {
        bool isOkay = CheckAbilityToUpdate();
        if (!isOkay)
        {
            MessageBox.Show("Fix all of the errors first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            UpdateSubscriptionButton.IsEnabled = false;
            return;
        }

        DateTime startDate = StartDatePicker.Value ?? DateTime.Now;
        DateTime endDate = EndDatePicker.Value ?? DateTime.Now.AddDays(30);
        Client selectedClient = (Client)ClientComboBox.SelectedItem;
        SubscriptionType selectedSubscriptionType = (SubscriptionType)SubscriptionTypeComboBox.SelectedItem;

        await Database.UpdateSubscription(
            _subscriptionID,
            selectedSubscriptionType.SubscriptionTypeId,
            float.Parse(PriceTextBox.Text),
            startDate,
            endDate,
            selectedClient.ClientId);

        MainWindow.MainWindowViewModel.Subscriptions = await Database.GetAllSubscriptions();
        Close();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => CheckAbilityToUpdate();

    private bool CheckAbilityToUpdate()
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

        UpdateSubscriptionButton.IsEnabled = isOkay;
        return isOkay;
    }

    private async void DeleteSubscriptionButton_Click(object sender, RoutedEventArgs e)
    {
        if (_subscription == null) return;

        MessageBoxResult result = MessageBox.Show(
            $"Are you sure you want to delete subscription {_subscription.SubscriptionTypeName} for client {_subscription.ClientId}?",
            "Deleting Subscription",
            MessageBoxButton.YesNo,
            MessageBoxImage.Information);

        if (result == MessageBoxResult.Yes)
        {
            await Database.DeleteSubscription(_subscriptionID);
            MainWindow.MainWindowViewModel.Subscriptions = await Database.GetAllSubscriptions();
            Close();
        }
    }
}