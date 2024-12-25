using Swimming_Pool.Models;
using System.Windows;
using System.Windows.Controls;

namespace Swimming_Pool.Views;

public partial class UpdateSpecializationTypeWindow : Window
{
    private SpecializationType? _subscriptionType;
    private int _subscriptionTypeID = -1;

    public UpdateSpecializationTypeWindow()
    {
        InitializeComponent();
    }

    public async void Initialize(int subscriptionTypeID)
    {
        _subscriptionTypeID = subscriptionTypeID;
        _subscriptionType = await Database.GetSpecializationTypeById(subscriptionTypeID);
        if (_subscriptionType == null)
        {
            return;
        }
        SpecializationTypeNameTextBox.Text = _subscriptionType.Specialization;
    }

    private async void CancelUpdatingButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.MainWindowViewModel.Subscriptions = await Database.GetAllSubscriptions();
        Close();
    }

    private async void UpdateSpecializationTypeButton_Click(object sender, RoutedEventArgs e)
    {
        bool isOkay = CheckAbilityToUpdate();
        if (!isOkay)
        {
            MessageBox.Show("Fix all of the errors first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            UpdateSpecializationTypeButton.IsEnabled = false;
            return;
        }

        await Database.UpdateSpecializationType(_subscriptionTypeID, SpecializationTypeNameTextBox.Text);

        MainWindow.MainWindowViewModel.SpecializationTypes = await Database.GetAllSpecializationTypes();
        Close();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => CheckAbilityToUpdate();

    private bool CheckAbilityToUpdate()
    {
        bool isOkay = true;

        if (string.IsNullOrWhiteSpace(SpecializationTypeNameTextBox.Text))
        {
            isOkay = false;
        }

        UpdateSpecializationTypeButton.IsEnabled = isOkay;
        return isOkay;
    }

    private async void DeleteSpecializationTypeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_subscriptionType == null) return;

        MessageBoxResult result = MessageBox.Show(
            $"Are you sure you want to delete specialization type {_subscriptionType.Specialization}?",
            "Deleting Specialization Type",
            MessageBoxButton.YesNo,
            MessageBoxImage.Information);

        if (result == MessageBoxResult.Yes)
        {
            await Database.DeleteSpecializationType(_subscriptionTypeID);
            MainWindow.MainWindowViewModel.SpecializationTypes = await Database.GetAllSpecializationTypes();
            Close();
        }
    }
}