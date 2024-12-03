using Swimming_Pool.Models;
using Swimming_Pool.ViewModels;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Swimming_Pool.Views;

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
            MainWindowViewModel.InstructorsWithNull = [..MainWindowViewModel.Instructors];
            Instructor nullInstructor = new() { InstructorId = -1, FirstName = "Прибрати інструктора" };
            MainWindowViewModel.InstructorsWithNull.Add(nullInstructor);

            MainWindowViewModel.Subscriptions = await Database.GetAllSubscriptions();

            MainWindowViewModel.Pools = await Database.GetAllPools();
            MainWindowViewModel.PoolsWithNull = [..MainWindowViewModel.Pools];
            Pool nullPool = new() { PoolId = -1, Name = "Прибрати басейн" };
            MainWindowViewModel.PoolsWithNull.Add(nullPool);

            SelectItemById(FilterTrainingInstructorComboBox, nullInstructor, i => i!.InstructorId);
            SelectItemById(FilterTrainingPoolComboBox, nullPool, i => i!.PoolId);

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
                                training.PoolId,
                                training.InstructorId
                            );
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

                            await Database.UpdateTraining(
                                training.TrainingId,
                                training.Date,
                                valueNew,
                                training.PoolId,
                                training.InstructorId
                            );
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

                            await Database.UpdateTraining(
                                training.TrainingId,
                                training.Date,
                                training.TrainingType!,
                                training.PoolId,
                                training.InstructorId
                            );
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
            MainWindowViewModel.SelectedInstructorId = training.InstructorId;
        }
    }

    private async void ComboBoxClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is Client && MainWindowViewModel.SelectedTraining != null)
        {
            Training t = MainWindowViewModel.SelectedTraining;

            await Database.UpdateTraining(
                t.TrainingId,
                t.Date,
                t.TrainingType!,
                t.PoolId,
                t.InstructorId
            );
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

            await Database.UpdateTraining(
                t.TrainingId,
                t.Date,
                t.TrainingType!,
                t.PoolId,
                t.InstructorId
            );
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
        StatisticsGrid.Visibility = Visibility.Collapsed;
        QueryEditorGrid.Visibility = Visibility.Visible;
        MenuItemQueryEditor.IsEnabled = false;
        MenuItemStatistics.IsEnabled = true;
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
        MenuItemStatistics.IsEnabled = true;
        TabControlView.Visibility = Visibility.Visible;
        QueryEditorGrid.Visibility = Visibility.Collapsed;
        StatisticsGrid.Visibility = Visibility.Collapsed;
    }

    private void ToggleClientFilterButton_Click(object sender, RoutedEventArgs e)
    {
        if ((bool)FilterClientButton.IsChecked!)
        {
            FilterClientBlock.Visibility = Visibility.Visible;
            return;
        }
        FilterClientBlock.Visibility = Visibility.Collapsed;
    }

    private async void ApplyFilterClientButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindowViewModel.Clients = await Database.GetClientsFiltered(FilterClientFirstName.Text, FilterClientLastName.Text, FilterClientAge.Text, 
            FilterClientPhoneNumber.Text, FilterClientEmailAddress.Text);
    }

    private void ToggleInstructorFilterButton_Click(object sender, RoutedEventArgs e)
    {
        if ((bool)FilterInstructorButton.IsChecked!)
        {
            FilterInstructorBlock.Visibility = Visibility.Visible;
            return;
        }
        FilterInstructorBlock.Visibility = Visibility.Collapsed;
    }

    private async void ApplyFilterInstructorButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindowViewModel.Instructors = await Database.GetInstructorsFiltered(FilterInstructorFirstName.Text, FilterInstructorLastName.Text, FilterInstructorAge.Text,
            FilterInstructorPhoneNumber.Text, FilterInstructorEmailAddress.Text, FilterInstructorSpecialization.Text);
    }

    private void ToggleTrainingFilterButton_Click(object sender, RoutedEventArgs e)
    {
        if ((bool)FilterTrainingButton.IsChecked!)
        {
            FilterTrainingBlock.Visibility = Visibility.Visible;
            return;
        }
        FilterTrainingBlock.Visibility = Visibility.Collapsed;
    }

    private async void ApplyFilterTrainingButton_Click(object sender, RoutedEventArgs e)
    {
        Instructor? instructor = (Instructor)FilterTrainingInstructorComboBox.SelectedItem;
        Pool? pool = (Pool)FilterTrainingPoolComboBox.SelectedItem;
        MainWindowViewModel.Trainings = await Database.GetTrainingFiltered(
            FilterTrainingYear.Value,
            FilterTrainingMonth.Value,
            FilterTrainingDay.Value,
            FilterTrainingTrainingType.Text,
            FilterTrainingClientNames.Text,
            pool?.PoolId ?? -1,
            instructor?.InstructorId ?? -1
        );
    }

    public static void SelectItemById<T>(ComboBox comboBox, T targetItem, Func<T, int> idSelector)
    {
        if (comboBox.ItemsSource == null || targetItem == null) return;
        int targetId = idSelector(targetItem);
        int index = comboBox.ItemsSource.Cast<T>().ToList().FindIndex(item => idSelector(item) == targetId);
        comboBox.SelectedIndex = index >= 0 ? index : -1;
    }

    private async void MenuItemAppStatistics_Click(object sender, RoutedEventArgs e)
    {
        TabControlView.Visibility = Visibility.Collapsed;
        StatisticsGrid.Visibility = Visibility.Visible;
        QueryEditorGrid.Visibility = Visibility.Collapsed;
        MenuItemStatistics.IsEnabled = false;
        MenuItemQueryEditor.IsEnabled = true;
        TextBlockResultAgeClient.Text = await Database.CalculateClientAgeStatistics();
        TextBlockResultAgeInstructor.Text = await Database.CalculateInstructorAgeStatistics();
    }

    private async void ComboBoxStatisticsNumClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Instructor instructor = (Instructor)StatisticsComboBoxNumClients.SelectedItem;
        ResultNumClientsTextBlock.Text = await Database.CalculateNumClientStatistics(instructor?.InstructorId ?? -1);
    }

    private async void ComboBoxMonthStatistics_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ResultNumOfTrainingsPerMonthTextBlock.Text = await Database.CalculateNumTrainingsPerMonth(ComboBoxMonthStatistics.SelectedIndex + 1);
    }

    private void ToggleSubscriptionFilterButton_Click(object sender, RoutedEventArgs e)
    {
        if ((bool)FilterSubscriptionButton.IsChecked!)
        {
            FilterSubscriptionBlock.Visibility = Visibility.Visible;
            return;
        }
        FilterSubscriptionBlock.Visibility = Visibility.Collapsed;
    }

    private async void ApplyFilterSubscriptionButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindowViewModel.Subscriptions = await Database.GetSubscriptionsFiltered(
            FilterSubscriptionType.Text,
            FilterSubscriptionVisitCount.Text,
            FilterSubscriptionPrice.Text,
            FilterSubscriptionStartDate.Text,
            FilterSubscriptionEndDate.Text,
            FilterSubscriptionClientName.Text);
    }

    private async void DataGridSubscription_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            DataGridRow row = e.Row;
            Subscription subscription = (Subscription)row.Item;

            if (e.EditingElement is TextBox tb)
            {
                string valueNew = tb.Text;

                switch (e.Column.Header.ToString())
                {
                    case "Type":
                        if (!string.IsNullOrWhiteSpace(valueNew))
                        {
                            subscription.SubscriptionType = valueNew;
                        }
                        else
                        {
                            tb.Text = subscription.SubscriptionType ?? string.Empty;
                        }
                        break;
                    case "Visit Count":
                        if (int.TryParse(valueNew, out int visits))
                        {
                            subscription.VisitCount = visits;
                        }
                        else
                        {
                            tb.Text = subscription.VisitCount.ToString();
                        }
                        break;
                    case "Price":
                        if (float.TryParse(valueNew, out float price))
                        {
                            subscription.Price = price;
                        }
                        else
                        {
                            tb.Text = subscription.Price.ToString("F2");
                        }
                        break;
                    case "Start Date":
                        if (DateTime.TryParse(valueNew, out DateTime startDate))
                        {
                            subscription.StartDate = startDate;
                        }
                        else
                        {
                            tb.Text = subscription.StartDate.ToString("yyyy-MM-dd");
                        }
                        break;
                    case "End Date":
                        if (DateTime.TryParse(valueNew, out DateTime endDate))
                        {
                            subscription.EndDate = endDate;
                        }
                        else
                        {
                            tb.Text = subscription.EndDate.ToString("yyyy-MM-dd");
                        }
                        break;
                }

                // Call Database.UpdateSubscription with updated parameters
                await Database.UpdateSubscription(
                    subscription.SubscriptionId,
                    subscription.SubscriptionType ?? string.Empty,
                    subscription.VisitCount,
                    subscription.Price,
                    subscription.StartDate,
                    subscription.EndDate,
                    subscription.ClientId);
            }
        }
    }

    private async void DataGridSubscription_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete)
        {
            if (sender is DataGrid dataGrid)
            {
                foreach (object? row in dataGrid.SelectedItems)
                {
                    if (row is Subscription subscription)
                    {
                        await Database.DeleteSubscription(subscription.SubscriptionId);
                    }
                    break;
                }
            }
        }
    }

    private void MenuItemSubscription_Click(object sender, RoutedEventArgs e)
    {
        CreateSubscriptionWindow createSubscriptionWindow = new()
        {
            Owner = this
        };
        createSubscriptionWindow.ShowDialog();
    }

    private void TogglePoolFilterButton_Click(object sender, RoutedEventArgs e)
    {
        if ((bool)FilterPoolButton.IsChecked!)
        {
            FilterPoolBlock.Visibility = Visibility.Visible;
            return;
        }
        FilterPoolBlock.Visibility = Visibility.Collapsed;
    }

    private async void ApplyFilterPoolButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindowViewModel.Pools = await Database.GetPoolsFiltered(
            FilterPoolName.Text,
            FilterPoolLaneCount.Text,
            FilterPoolLength.Text,
            FilterPoolDepth.Text,
            FilterPoolAddress.Text);
    }

    private async void DataGridPool_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            DataGridRow row = e.Row;
            Pool pool = (Pool)row.Item;
            if (e.EditingElement is TextBox tb)
            {
                string valueNew = tb.Text;
                switch (e.Column.Header.ToString())
                {
                    case "Name":
                        if (!string.IsNullOrWhiteSpace(valueNew))
                        {
                            pool.Name = valueNew;
                            await Database.UpdatePool(pool);
                        }
                        else
                        {
                            tb.Text = pool.Name;
                        }
                        break;
                    case "Lane Count":
                        if (int.TryParse(valueNew, out int lanes))
                        {
                            pool.LaneCount = lanes;
                            await Database.UpdatePool(pool);
                        }
                        else
                        {
                            tb.Text = pool.LaneCount.ToString();
                        }
                        break;
                    case "Length":
                        if (float.TryParse(valueNew, out float length))
                        {
                            pool.Length = length;
                            await Database.UpdatePool(pool);
                        }
                        else
                        {
                            tb.Text = pool.Length.ToString("F2");
                        }
                        break;
                    case "Depth":
                        if (float.TryParse(valueNew, out float depth))
                        {
                            pool.Depth = depth;
                            await Database.UpdatePool(pool);
                        }
                        else
                        {
                            tb.Text = pool.Depth.ToString("F2");
                        }
                        break;
                    case "Address":
                        if (!string.IsNullOrWhiteSpace(valueNew))
                        {
                            pool.Address = valueNew;
                            await Database.UpdatePool(pool);
                        }
                        else
                        {
                            tb.Text = pool.Address;
                        }
                        break;
                }
            }
        }
    }

    private async void DataGridPool_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete)
        {
            if (sender is DataGrid dataGrid)
            {
                foreach (object? row in dataGrid.SelectedItems)
                {
                    if (row is Pool pool)
                    {
                        await Database.DeletePool(pool.PoolId);
                    }
                    break;
                }
            }
        }
    }

    private void MenuItemPool_Click(object sender, RoutedEventArgs e)
    {
        CreatePoolWindow createPoolWindow = new()
        {
            Owner = this
        };
        createPoolWindow.ShowDialog();
    }

}