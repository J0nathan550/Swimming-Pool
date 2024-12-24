using Swimming_Pool.Models;
using Swimming_Pool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Swimming_Pool.Views;

public partial class CreateSubscriptionTypeWindow : Window
{
    public CreateSubscriptionTypeWindow()
    {
        InitializeComponent();
    }

    private async void CancelCreationButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.MainWindowViewModel.SubscriptionTypes = await Database.GetAllSubscriptionTypes();
        Close();
    }

    private async void CreateSubscriptionTypeButton_Click(object sender, RoutedEventArgs e)
    {
        bool isOkay = CheckAbilityToCreate();
        if (!isOkay)
        {
            MessageBox.Show("Fix all of the errors first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            CreateSubscriptionTypeButton.IsEnabled = false;
            return;
        }

        await Database.CreateSubscriptionType(SubscriptionTypeNameTextBox.Text, DescriptionTextBox.Text);

        MainWindow.MainWindowViewModel.SubscriptionTypes = await Database.GetAllSubscriptionTypes();
        Close();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => CheckAbilityToCreate();

    private bool CheckAbilityToCreate()
    {
        bool isOkay = true;

        if (string.IsNullOrWhiteSpace(SubscriptionTypeNameTextBox.Text))
        {
            isOkay = false;
        }

        if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
        {
            isOkay = false;
        }

        CreateSubscriptionTypeButton.IsEnabled = isOkay;
        return isOkay;
    }
}