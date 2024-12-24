using Swimming_Pool.Models;
using System.Windows;
using System.Windows.Controls;

namespace Swimming_Pool.Views;

public partial class UpdateSubscriptionTypeWindow : Window
{
    private SubscriptionType? _subscriptionType;
    private int _subscriptionTypeID = -1;

    public UpdateSubscriptionTypeWindow()
    {
        InitializeComponent();
    }

    public async void Initialize(int subscriptionTypeID)
    {
        _subscriptionTypeID = subscriptionTypeID;
        _subscriptionType = await Database.GetSubscriptionTypeById(subscriptionTypeID);
        if (_subscriptionType == null)
        {
            return;
        }
        NameTextBox.Text = _subscriptionType.Name;
        DescriptionTextBox.Text = _subscriptionType.Description;
    }

    private async void CancelUpdatingButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.MainWindowViewModel.Subscriptions = await Database.GetAllSubscriptions();
        Close();
    }

    private async void UpdateSubscriptionTypeButton_Click(object sender, RoutedEventArgs e)
    {
        bool isOkay = CheckAbilityToUpdate();
        if (!isOkay)
        {
            MessageBox.Show("Fix all of the errors first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            UpdateSubscriptionTypeButton.IsEnabled = false;
            return;
        }

        await Database.UpdateSubscriptionType(_subscriptionTypeID, NameTextBox.Text, DescriptionTextBox.Text);

        MainWindow.MainWindowViewModel.SubscriptionTypes = await Database.GetAllSubscriptionTypes();
        Close();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => CheckAbilityToUpdate();

    private bool CheckAbilityToUpdate()
    {
        bool isOkay = true;

        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            isOkay = false;
        }
        if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
        {
            isOkay = false;
        }

        UpdateSubscriptionTypeButton.IsEnabled = isOkay;
        return isOkay;
    }

    private async void DeleteSubscriptionTypeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_subscriptionType == null) return;

        MessageBoxResult result = MessageBox.Show(
            $"Are you sure you want to delete subscription type {_subscriptionType.Name}?",
            "Deleting Subscription Type",
            MessageBoxButton.YesNo,
            MessageBoxImage.Information);

        if (result == MessageBoxResult.Yes)
        {
            await Database.DeleteSubscriptionType(_subscriptionTypeID);
            MainWindow.MainWindowViewModel.SubscriptionTypes = await Database.GetAllSubscriptionTypes();
            Close();
        }
    }
}