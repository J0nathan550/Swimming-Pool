using System.Windows;
using System.Windows.Controls;

namespace Swimming_Pool.Views;

public partial class CreateSpecializationTypeWindow : Window
{
    public CreateSpecializationTypeWindow() => InitializeComponent();

    private async void CancelCreationButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.MainWindowViewModel.SpecializationTypes = await Database.GetAllSpecializationTypes();
        Close();
    }

    private async void CreateSpecializationTypeButton_Click(object sender, RoutedEventArgs e)
    {
        bool isOkay = CheckAbilityToCreate();
        if (!isOkay)
        {
            MessageBox.Show("Fix all of the errors first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            CreateSpecializationTypeButton.IsEnabled = false;
            return;
        }

        await Database.CreateSpecializationType(SpecializationTypeNameTextBox.Text);

        MainWindow.MainWindowViewModel.SpecializationTypes = await Database.GetAllSpecializationTypes();
        Close();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => CheckAbilityToCreate();

    private bool CheckAbilityToCreate()
    {
        bool isOkay = true;

        if (string.IsNullOrWhiteSpace(SpecializationTypeNameTextBox.Text))
        {
            isOkay = false;
        }

        CreateSpecializationTypeButton.IsEnabled = isOkay;
        return isOkay;
    }
}