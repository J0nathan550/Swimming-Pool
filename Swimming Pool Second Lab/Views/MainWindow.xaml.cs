﻿using Swimming_Pool_Second_Lab.Models;
using Swimming_Pool_Second_Lab.ViewModels;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Swimming_Pool_Second_Lab.Views;

public partial class MainWindow : Window
{
    private static MainWindowViewModel mainWindowViewModel = new();

    public static MainWindowViewModel MainWindowViewModel { get => mainWindowViewModel; set => mainWindowViewModel = value; }
    public static MainWindow? MainWindowInstance { get => mainWindow; set => mainWindow = value; }

    private static MainWindow? mainWindow;

    public MainWindow()
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        DataContext = MainWindowViewModel;
        InitializeComponent();
        MainWindowInstance = this;
    }

    private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is TabItem)
        {
            MainWindowViewModel.Clients = await Database.GetAllClients();
            MainWindowViewModel.Instructors = await Database.GetAllInstructors();
            MainWindowViewModel.Trainings = await Database.GetAllTrainings();
        }
    }

    private async void DataGridClient_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            DataGridRow row = e.Row;
            Client client = (Client)row.Item;
            if (e.EditingElement is TextBox tb)
            {
                string valueNew = tb.Text;
                switch (e.Column.Header.ToString())
                {
                    case "Age":
                        if (int.TryParse(valueNew, out int actualAgeValue))
                        {
                            client.Age = actualAgeValue;
                            await Database.UpdateClient(client.ClientId, client.FirstName!, client.LastName!, client.Age, client.PhoneNumber!, client.EmailAddress!);
                        }
                        else
                        {
                            tb.Text = client.Age.ToString();
                        }
                        break;
                    case "First Name":
                        if (!string.IsNullOrWhiteSpace(valueNew))
                        {
                            client.FirstName = valueNew;
                            await Database.UpdateClient(client.ClientId, client.FirstName, client.LastName!, client.Age, client.PhoneNumber!, client.EmailAddress!);
                        }
                        else
                        {
                            tb.Text = client.FirstName;
                        }
                        break;
                    case "Last Name":
                        if (!string.IsNullOrWhiteSpace(valueNew))
                        {
                            client.LastName = valueNew;
                            await Database.UpdateClient(client.ClientId, client.FirstName!, client.LastName, client.Age, client.PhoneNumber!, client.EmailAddress!);
                        }
                        else
                        {
                            tb.Text = client.LastName;
                        }
                        break;
                    case "Phone Number":
                        if (!string.IsNullOrWhiteSpace(valueNew))
                        {
                            client.PhoneNumber = valueNew;
                            await Database.UpdateClient(client.ClientId, client.FirstName!, client.LastName!, client.Age, client.PhoneNumber, client.EmailAddress!);
                        }
                        else
                        {
                            tb.Text = client.PhoneNumber;
                        }
                        break;
                    case "Email Address":
                        if (!string.IsNullOrWhiteSpace(valueNew))
                        {
                            client.EmailAddress = valueNew;
                            await Database.UpdateClient(client.ClientId, client.FirstName!, client.LastName!, client.Age, client.PhoneNumber!, client.EmailAddress);
                        }
                        else
                        {
                            tb.Text = client.EmailAddress;
                        }
                        break;
                }
            }
        }
    }

    private async void DataGridClient_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete)
        {
            if (sender is DataGrid dataGrid)
            {
                foreach (object? row in dataGrid.SelectedItems)
                {
                    if (row is Client client)
                    {
                        await Database.DeleteClient(client.ClientId);
                    }
                    break;
                }
            }
        }
    }

    private void MenuItemClient_Click(object sender, RoutedEventArgs e)
    {
        CreateClientWindow createClientWindow = new()
        {
            Owner = this
        };
        createClientWindow.ShowDialog();
    }

    private void MenuItemTraining_Click(object sender, RoutedEventArgs e)
    {
        CreateTrainingWindow createTrainingWindow = new()
        {
            Owner = this
        };
        createTrainingWindow.ShowDialog();
    }

    private async void DataGridTraining_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            DataGridRow row = e.Row;
            Training training = (Training)row.Item;
            if (e.EditingElement is TextBox tb)
            {
                string valueNew = tb.Text;
                switch (e.Column.Header.ToString())
                {
                    case "Date":
                        if (DateTime.TryParseExact(valueNew, "yyyy-MM-dd HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime actualDateValue))
                        {
                            training.Date = actualDateValue;

                            await Database.UpdateTraining(
                                training.TrainingId,
                                actualDateValue,
                                training.TrainingType ?? string.Empty,
                                training.PoolName ?? string.Empty,
                                training.ClientId,
                                training.InstructorId);
                        }
                        else
                        {
                            tb.Text = training.Date.ToString("yyyy-MM-dd HH:mm");
                        }
                        break;
                    case "Training Type":
                        if (!string.IsNullOrWhiteSpace(valueNew))
                        {
                            training.TrainingType = valueNew;
                            await Database.UpdateTraining(training.TrainingId, training.Date, valueNew, training.PoolName!, training.ClientId, training.InstructorId);
                        }
                        else
                        {
                            tb.Text = training.TrainingType;
                        }
                        break;
                    case "Pool Name":
                        if (!string.IsNullOrWhiteSpace(valueNew))
                        {
                            training.PoolName = valueNew;
                            await Database.UpdateTraining(training.TrainingId, training.Date, training.TrainingType!, valueNew, training.ClientId, training.InstructorId);
                        }
                        else
                        {
                            tb.Text = training.PoolName;
                        }
                        break;
                }
            }
        }
    }

    private async void DataGridTraining_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete)
        {
            if (sender is DataGrid dataGrid)
            {
                foreach (object? row in dataGrid.SelectedItems)
                {
                    if (row is Training training)
                    {
                        await Database.DeleteTraining(training.TrainingId);
                    }
                    break;
                }
            }
        }
    }

    private void DataGridTraining_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is Training training)
        {
            MainWindowViewModel.SelectedTraining = training;
            MainWindowViewModel.SelectedClientId = training.ClientId;
            MainWindowViewModel.SelectedInstructorId = training.InstructorId;
        }
    }

    private async void ComboBoxClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is Client client && MainWindowViewModel.SelectedTraining != null)
        {
            Training t = MainWindowViewModel.SelectedTraining;
            t.ClientId = client.ClientId;
            t.ClientName = client.FirstName;
            await Database.UpdateTraining(t.TrainingId, t.Date, t.TrainingType!, t.PoolName!, t.ClientId, t.InstructorId);
            MainWindowViewModel.Trainings = await Database.GetAllTrainings();
        }
    }

    private async void ComboBoxInstructor_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is Instructor instructor && MainWindowViewModel.SelectedTraining != null)
        {
            Training t = MainWindowViewModel.SelectedTraining;
            t.InstructorId = instructor.InstructorId;
            t.InstructorName = instructor.FirstName;
            await Database.UpdateTraining(t.TrainingId, t.Date, t.TrainingType!, t.PoolName!, t.ClientId, t.InstructorId);
            MainWindowViewModel.Trainings = await Database.GetAllTrainings();
        }
    }

    private async void DataGridInstructor_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete)
        {
            if (sender is DataGrid dataGrid)
            {
                foreach (object? row in dataGrid.SelectedItems)
                {
                    if (row is Instructor instructor)
                    {
                        await Database.DeleteInstructor(instructor.InstructorId);
                    }
                    break;
                }
            }
        }
    }

    private async void DataGridInstructor_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            DataGridRow row = e.Row;
            Instructor instructor = (Instructor)row.Item;
            if (e.EditingElement is TextBox tb)
            {
                string valueNew = tb.Text;
                switch (e.Column.Header.ToString())
                {
                    case "Age":
                        if (int.TryParse(valueNew, out int actualAgeValue))
                        {
                            instructor.Age = actualAgeValue;
                            await Database.UpdateInstructor(instructor.InstructorId, instructor.FirstName!, instructor.LastName!, instructor.Age, instructor.PhoneNumber!, instructor.EmailAddress!, instructor.Specialization!);
                        }
                        else
                        {
                            tb.Text = instructor.Age.ToString();
                        }
                        break;
                    case "First Name":
                        if (!string.IsNullOrWhiteSpace(valueNew))
                        {
                            instructor.FirstName = valueNew;
                            await Database.UpdateInstructor(instructor.InstructorId, instructor.FirstName, instructor.LastName!, instructor.Age, instructor.PhoneNumber!, instructor.EmailAddress!, instructor.Specialization!);
                        }
                        else
                        {
                            tb.Text = instructor.FirstName;
                        }
                        break;
                    case "Last Name":
                        if (!string.IsNullOrWhiteSpace(valueNew))
                        {
                            instructor.LastName = valueNew;
                            await Database.UpdateInstructor(instructor.InstructorId, instructor.FirstName!, instructor.LastName, instructor.Age, instructor.PhoneNumber!, instructor.EmailAddress!, instructor.Specialization!);
                        }
                        else
                        {
                            tb.Text = instructor.LastName;
                        }
                        break;
                    case "Phone Number":
                        if (!string.IsNullOrWhiteSpace(valueNew))
                        {
                            instructor.PhoneNumber = valueNew;
                            await Database.UpdateInstructor(instructor.InstructorId, instructor.FirstName!, instructor.LastName!, instructor.Age, instructor.PhoneNumber, instructor.EmailAddress!, instructor.Specialization!);
                        }
                        else
                        {
                            tb.Text = instructor.PhoneNumber;
                        }
                        break;
                    case "Email Address":
                        if (!string.IsNullOrWhiteSpace(valueNew))
                        {
                            instructor.EmailAddress = valueNew;
                            await Database.UpdateInstructor(instructor.InstructorId, instructor.FirstName!, instructor.LastName!, instructor.Age, instructor.PhoneNumber!, instructor.EmailAddress, instructor.Specialization!);
                        }
                        else
                        {
                            tb.Text = instructor.EmailAddress;
                        }
                        break;
                    case "Specialization":
                        if (!string.IsNullOrWhiteSpace(valueNew))
                        {
                            instructor.Specialization = valueNew;
                            await Database.UpdateInstructor(instructor.InstructorId, instructor.FirstName!, instructor.LastName!, instructor.Age, instructor.PhoneNumber!, instructor.EmailAddress!, valueNew);
                        }
                        else
                        {
                            tb.Text = instructor.Specialization;
                        }
                        break;
                }
            }
        }
    }

    private void MenuItemInstructor_Click(object sender, RoutedEventArgs e)
    {
        CreateInstructorWindow createInstructorWindow = new()
        {
            Owner = this
        };
        createInstructorWindow.ShowDialog();
    }

    private void MenuItemApp_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

    private void MenuItemAppQueryEditor_Click(object sender, RoutedEventArgs e)
    {
        TabControlView.Visibility = Visibility.Collapsed;
        QueryEditorGrid.Visibility = Visibility.Visible;
        MenuItemQueryEditor.IsEnabled = false;
        ClearSQLBox_Click(sender, e);
        ExecuteSQLButton_Click(sender, e);
    }

    private async void ExecuteSQLButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string sqlQuery = SQLQueryTextBox.Text;
            IEnumerable<dynamic> result = await Database.ExecuteQuery(sqlQuery);
            QueryResultDataGrid.Columns.Clear();
            QueryResultDataGrid.ItemsSource = result;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error in Query Editor!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ClearSQLBox_Click(object sender, RoutedEventArgs e)
    {
        SQLQueryTextBox.Text = "SELECT * FROM client";
        ExecuteSQLButton_Click(sender, e);
    }

    private void ExitQueryEditor_Click(object sender, RoutedEventArgs e)
    {
        MenuItemQueryEditor.IsEnabled = true;
        TabControlView.Visibility = Visibility.Visible;
        QueryEditorGrid.Visibility = Visibility.Collapsed;
    }
}