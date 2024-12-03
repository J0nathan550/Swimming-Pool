using Swimming_Pool.Models;
using System.Windows;
using System.Windows.Controls;

namespace Swimming_Pool.Views;

public partial class UpdateInstructorWindow : Window
{
    private Instructor? _instructor;
    private int _instructorID = -1;

    public UpdateInstructorWindow() => InitializeComponent();

    public async void Initialize(int clientID)
    {
        _instructorID = clientID;
        _instructor = await Database.GetInstructorById(clientID);
        FirstNameTextBox.Text = _instructor!.FirstName;
        LastNameTextBox.Text = _instructor.LastName;
        AgeTextBox.Value = _instructor.Age;
        EmailAddressTextBox.Text = _instructor.EmailAddress;
        PhoneNumberTextBox.Text = _instructor.PhoneNumber;
        SpecializationTextBox.Text = _instructor.Specialization;
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
        await Database.UpdateInstructor(_instructorID, FirstNameTextBox.Text, LastNameTextBox.Text, int.Parse(AgeTextBox.Text), PhoneNumberTextBox.Text, EmailAddressTextBox.Text, SpecializationTextBox.Text);
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
            if (!int.TryParse(AgeTextBox.Text, out int age))
            {
                isOkay = false;
            }
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
}