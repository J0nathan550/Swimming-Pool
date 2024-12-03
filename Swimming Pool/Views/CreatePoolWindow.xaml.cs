using System.Windows;
using System.Windows.Controls;

namespace Swimming_Pool.Views;

public partial class CreatePoolWindow : Window
{
    public CreatePoolWindow() => InitializeComponent();

    private async void CancelCreationButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.MainWindowViewModel.Pools = await Database.GetAllPools();
        Close();
    }

    private async void CreatePoolButton_Click(object sender, RoutedEventArgs e)
    {
        bool isOkay = CheckAbilityToCreate();
        if (!isOkay)
        {
            MessageBox.Show("Fix all of the errors first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            CreatePoolButton.IsEnabled = false;
            return;
        }

        await Database.CreatePool(
            PoolNameTextBox.Text,
            int.Parse(LaneCountTextBox.Text),
            float.Parse(LengthTextBox.Text),
            float.Parse(DepthTextBox.Text),
            AddressTextBox.Text);

        MainWindow.MainWindowViewModel.Pools = await Database.GetAllPools();
        Close();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => CheckAbilityToCreate();

    private bool CheckAbilityToCreate()
    {
        bool isOkay = true;

        if (string.IsNullOrWhiteSpace(PoolNameTextBox.Text))
        {
            isOkay = false;
        }

        if (string.IsNullOrWhiteSpace(AddressTextBox.Text))
        {
            isOkay = false;
        }

        if (string.IsNullOrWhiteSpace(LaneCountTextBox.Text) ||
            !int.TryParse(LaneCountTextBox.Text, out int _))
        {
            isOkay = false;
        }

        if (string.IsNullOrWhiteSpace(LengthTextBox.Text) ||
            !float.TryParse(LengthTextBox.Text, out float _))
        {
            isOkay = false;
        }

        if (string.IsNullOrWhiteSpace(DepthTextBox.Text) ||
            !float.TryParse(DepthTextBox.Text, out float _))
        {
            isOkay = false;
        }

        CreatePoolButton.IsEnabled = isOkay;
        return isOkay;
    }
}