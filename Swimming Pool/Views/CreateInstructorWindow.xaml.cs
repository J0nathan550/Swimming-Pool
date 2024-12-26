using Swimming_Pool.Models;
using Swimming_Pool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Swimming_Pool.Views;

public partial class CreateInstructorWindow : Window
{
    private readonly CreateUpdateSpecializationViewModel _createUpdateSpecializationViewModel = new();

    public CreateInstructorWindow()
    {
        DataContext = _createUpdateSpecializationViewModel;
        InitializeComponent();
        Initialize();
    }

    private async void Initialize()
    {
        _createUpdateSpecializationViewModel.SpecializationTypes = await Database.GetAllSpecializationTypes();
    }

    private async void CancelCreationButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.MainWindowViewModel.Instructors = await Database.GetAllInstructors();
        Close();
    }

    private async void CreateInstructorButton_Click(object sender, RoutedEventArgs e)
    {
        bool isOkay = CheckAbilityToCreate();
        if (!isOkay)
        {
            MessageBox.Show("Fix all of the errors first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            CreateInstructorButton.IsEnabled = false;
            return;
        }

        SpecializationType? specializationType = (SpecializationType)SpecializationComboBox.SelectedItem;

        await Database.CreateInstructor(FirstNameTextBox.Text, LastNameTextBox.Text, int.Parse(AgeTextBox.Text), PhoneNumberTextBox.Text, EmailAddressTextBox.Text, specializationType.InstructorSpecializationId);
        MainWindow.MainWindowViewModel.Instructors = await Database.GetAllInstructors();
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
            if (!int.TryParse(AgeTextBox.Text, out _))
            {
                isOkay = false;
            }
        }

        if (SpecializationComboBox.SelectedItem == null)
        {
            isOkay = false;
        }

        CreateInstructorButton.IsEnabled = isOkay;
        return isOkay;
    }

    private async void TextBoxSpecializationSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        _createUpdateSpecializationViewModel.SpecializationTypes = await Database.GetSpecializationFilteredByName(SpecializationSearchTextBox.Text);
    }

    private void SearchButtonSpecialization_Click(object sender, RoutedEventArgs e)
    {
        if ((bool)SearchSpecializationToggleButton.IsChecked!)
        {
            SpecializationComboBox.Visibility = Visibility.Collapsed;
            SpecializationSearchTextBox.Visibility = Visibility.Visible;
            return;
        }
        SpecializationComboBox.Visibility = Visibility.Visible;
        SpecializationSearchTextBox.Visibility = Visibility.Collapsed;
    }

    private void SpecializationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CheckAbilityToCreate();
    }
}