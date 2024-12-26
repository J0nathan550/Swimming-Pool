using Swimming_Pool.Models;
using Swimming_Pool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Swimming_Pool.Views;

public partial class UpdateInstructorWindow : Window
{
    private readonly CreateUpdateSpecializationViewModel _createUpdateSpecializationViewModel = new();
    private Instructor? _instructor;
    private int _instructorID = -1;

    public UpdateInstructorWindow()
    {
        DataContext = _createUpdateSpecializationViewModel;
        InitializeComponent();
    }

    public async void Initialize(int clientID)
    {
        _instructorID = clientID;
        _instructor = await Database.GetInstructorById(clientID);
        _createUpdateSpecializationViewModel.SpecializationTypes = await Database.GetAllSpecializationTypes();
        SpecializationType? specializationType = await Database.GetSpecializationTypeById(_instructor.InstructorSpecializationId);
        SelectItemById(SpecializationComboBox, specializationType, s => s!.InstructorSpecializationId);
        FirstNameTextBox.Text = _instructor!.FirstName;
        LastNameTextBox.Text = _instructor.LastName;
        AgeTextBox.Value = _instructor.Age;
        EmailAddressTextBox.Text = _instructor.EmailAddress;
        PhoneNumberTextBox.Text = _instructor.PhoneNumber;
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
        MainWindow.MainWindowViewModel.Instructors = await Database.GetAllInstructors();
        Close();
    }

    private async void UpdateInstructorButton_Click(object sender, RoutedEventArgs e)
    {
        bool isOkay = CheckAbilityToUpdate();
        if (!isOkay)
        {
            MessageBox.Show("Fix all of the errors first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            UpdateInstructorButton.IsEnabled = false;
            return;
        }
        SpecializationType? specializationType = (SpecializationType)SpecializationComboBox.SelectedItem;
        await Database.UpdateInstructor(_instructorID, FirstNameTextBox.Text, LastNameTextBox.Text, int.Parse(AgeTextBox.Text), PhoneNumberTextBox.Text, EmailAddressTextBox.Text, specializationType.InstructorSpecializationId);
        MainWindow.MainWindowViewModel.Instructors = await Database.GetAllInstructors();
        Close();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => CheckAbilityToUpdate();

    private bool CheckAbilityToUpdate()
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

        UpdateInstructorButton.IsEnabled = isOkay;
        return isOkay;
    }

    private async void DeleteInstructorButton_Click(object sender, RoutedEventArgs e)
    {
        if (_instructor == null) return;
        MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete instructor: {_instructor.FirstName} {_instructor.LastName}?", "Deleting Instructor", MessageBoxButton.YesNo, MessageBoxImage.Error);
        if (result == MessageBoxResult.Yes)
        {
            await Database.DeleteInstructor(_instructorID);
            MainWindow.MainWindowViewModel.Instructors = await Database.GetAllInstructors();
            Close();
        }
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
        CheckAbilityToUpdate();
    }
}