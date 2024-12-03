using Swimming_Pool.Models;
using System.Windows;
using System.Windows.Controls;

namespace Swimming_Pool.Views;

public partial class UpdatePoolWindow : Window
{
    private Pool? _pool;
    private int _poolID = -1;

    public UpdatePoolWindow()
    {
        InitializeComponent();
    }

    public async void Initialize(int poolID)
    {
        // Load existing pool data
        _poolID = poolID;
        _pool = await Database.GetPoolById(poolID);

        // Pre-fill fields with pool data
        NameTextBox.Text = _pool!.Name;
        LaneCountTextBox.Text = _pool.LaneCount.ToString();
        LengthTextBox.Text = _pool.Length.ToString("F2");
        DepthTextBox.Text = _pool.Depth.ToString("F2");
        AddressTextBox.Text = _pool.Address;
    }

    private async void CancelUpdatingButton_Click(object sender, RoutedEventArgs e)
    {
        // Refresh the pool list and close
        MainWindow.MainWindowViewModel.Pools = await Database.GetAllPools();
        Close();
    }

    private async void UpdatePoolButton_Click(object sender, RoutedEventArgs e)
    {
        bool isOkay = CheckAbilityToUpdate();
        if (!isOkay)
        {
            MessageBox.Show("Fix all of the errors first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            UpdatePoolButton.IsEnabled = false;
            return;
        }

        // Collect updated pool data
        Pool updatedPool = new()
        {
            PoolId = _poolID,
            Name = NameTextBox.Text,
            LaneCount = int.Parse(LaneCountTextBox.Text),
            Length = float.Parse(LengthTextBox.Text),
            Depth = float.Parse(DepthTextBox.Text),
            Address = AddressTextBox.Text
        };

        // Update pool in the database
        await Database.UpdatePool(updatedPool);

        // Refresh the pool list and close
        MainWindow.MainWindowViewModel.Pools = await Database.GetAllPools();
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

        UpdatePoolButton.IsEnabled = isOkay;
        return isOkay;
    }

    private async void DeletePoolButton_Click(object sender, RoutedEventArgs e)
    {
        if (_pool == null) return;

        MessageBoxResult result = MessageBox.Show(
            $"Are you sure you want to delete the pool \"{_pool.Name}\"?",
            "Deleting Pool",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            await Database.DeletePool(_poolID);
            MainWindow.MainWindowViewModel.Pools = await Database.GetAllPools();
            Close();
        }
    }
}