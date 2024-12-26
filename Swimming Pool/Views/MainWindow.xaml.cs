using Microsoft.Win32;
using QuestPDF.Fluent;
using Swimming_Pool.Models;
using Swimming_Pool.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

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
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
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
            MainWindowViewModel.SubscriptionTypes = await Database.GetAllSubscriptionTypes();
            MainWindowViewModel.SpecializationTypes = await Database.GetAllSpecializationTypes();

            MainWindowViewModel.Pools = await Database.GetAllPools();
            MainWindowViewModel.PoolsWithNull = [..MainWindowViewModel.Pools];
            Pool nullPool = new() { PoolId = -1, Name = "Прибрати басейн" };
            MainWindowViewModel.PoolsWithNull.Add(nullPool);

            SelectItemById(FilterTrainingInstructorComboBox, nullInstructor, i => i!.InstructorId);
            SelectItemById(FilterTrainingPoolComboBox, nullPool, i => i!.PoolId);

            MainWindowViewModel.Trainings = await Database.GetAllTrainings();
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

    private void MenuItemInstructorSpecialization_Click(object sender, RoutedEventArgs e)
    {
        CreateSpecializationTypeWindow createSpecializationTypeWindow = new()
        {
            Owner = this
        };
        createSpecializationTypeWindow.ShowDialog();
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
            FilterSubscriptionPrice.Text,
            FilterSubscriptionStartDate.Text,
            FilterSubscriptionEndDate.Text,
            FilterSubscriptionClientName.Text);
    }

    private void MenuItemSubscription_Click(object sender, RoutedEventArgs e)
    {
        CreateSubscriptionWindow createSubscriptionWindow = new()
        {
            Owner = this
        };
        createSubscriptionWindow.ShowDialog();
    }

    private void MenuItemSubscriptionType_Click(object sender, RoutedEventArgs e)
    {
        CreateSubscriptionTypeWindow createSubscriptionTypeWindow = new()
        {
            Owner = this
        };
        createSubscriptionTypeWindow.ShowDialog();
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

    private void MenuItemPool_Click(object sender, RoutedEventArgs e)
    {
        CreatePoolWindow createPoolWindow = new()
        {
            Owner = this
        };
        createPoolWindow.ShowDialog();
    }

    private void MenuItemSubscriptionExportPDFType_Click(object sender, RoutedEventArgs e)
    {
        Document pdfDocument = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Header().Text("Список абонементів").FontSize(24).Bold().AlignCenter();
                page.Content().Column(col =>
                {
                    // Додавання інформації про абонементи
                    page.Margin(25);
                    foreach (SubscriptionType subscriptionType in mainWindowViewModel.SubscriptionTypes)
                    {
                        col.Item().Text($"Назва: {subscriptionType.Name}").FontSize(14);
                        col.Item().Text($"Опис: {subscriptionType.Description}").FontSize(14);
                        col.Item().Text("");
                    }
                });
                page.Footer().Text($"Дата створення документу: {DateTime.Now:yyyy.MM.dd HH:mm:ss}").AlignCenter();
            });
        });
        OpenFolderDialog openFileDialog = new()
        {
            Title = "Оберіть шлях експорту PDF за результатами Абонементів"
        };
        openFileDialog.ShowDialog();

        if (string.IsNullOrEmpty(openFileDialog.FolderName))
        {
            return;
        }

        pdfDocument.GeneratePdf(openFileDialog.FolderName + $"\\Абонементи - {DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf");

        MessageBoxResult result = MessageBox.Show("Файл було успішно створено!\nЧи хочете ви відчинити папку де знаходится файл?", "PDF Експорт - Абонементи", MessageBoxButton.YesNo, MessageBoxImage.Information);
        if (result == MessageBoxResult.Yes)
        {
            Process.Start("explorer.exe", openFileDialog.FolderName);
        }
    }

    private void MenuItemSubscriptionExportPDF_Click(object sender, RoutedEventArgs e)
    {
        Document pdfDocument = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Header().Text("Список абонентів, які купили підписку").FontSize(24).Bold().AlignCenter();
                page.Content().Column(col =>
                {
                    // Додавання інформації про абонементи
                    page.Margin(25);
                    foreach (Subscription subscription in mainWindowViewModel.Subscriptions)
                    {
                        col.Item().Text($"Тип: {subscription.SubscriptionTypeName}").FontSize(14);
                        col.Item().Text($"Ціна: {subscription.PriceAsString}").FontSize(14);
                        col.Item().Text($"Початок: {subscription.StartDate:dd.MM.yyyy}").FontSize(14);
                        col.Item().Text($"Кінець: {subscription.EndDate:dd.MM.yyyy}").FontSize(14);
                        col.Item().Text($"Клієнт: {subscription.ClientName}").FontSize(14);
                        col.Item().Text("");
                    }
                });
                page.Footer().Text($"Дата створення документу: {DateTime.Now:yyyy.MM.dd HH:mm:ss}").AlignCenter();
            });
        });
        OpenFolderDialog openFileDialog = new()
        {
            Title = "Оберіть шлях експорту PDF за результатами абонентів що купили підписку"
        };
        openFileDialog.ShowDialog();

        if (string.IsNullOrEmpty(openFileDialog.FolderName))
        {
            return;
        }

        pdfDocument.GeneratePdf(openFileDialog.FolderName + $"\\Список абонентів що купили підписку - {DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf");
       
        MessageBoxResult result = MessageBox.Show("Файл було успішно створено!\nЧи хочете ви відчинити папку де знаходится файл?", "PDF Експорт - Список абонентів що купили підписку", MessageBoxButton.YesNo, MessageBoxImage.Information);
        if (result == MessageBoxResult.Yes)
        {
            Process.Start("explorer.exe", openFileDialog.FolderName);
        }
    }

    private void MenuItemClientExportPDF_Click(object sender, RoutedEventArgs e)
    {
        Document pdfDocument = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Header().Text("Список клієнтів").FontSize(24).Bold().AlignCenter();
                page.Content().Column(col =>
                {
                    page.Margin(25);
                    foreach (Client client in mainWindowViewModel.Clients)
                    {
                        col.Item().Text($"Ім'я: {client.FirstName}").FontSize(14);
                        col.Item().Text($"Прізвище: {client.LastName}").FontSize(14);
                        col.Item().Text($"Вік: {client.Age}").FontSize(14);
                        col.Item().Text($"Телефон: {client.PhoneNumber}").FontSize(14);
                        col.Item().Text($"Електронна пошта: {client.EmailAddress}").FontSize(14);
                        col.Item().Text(""); 
                    }
                });
                page.Footer().Text($"Дата створення документу: {DateTime.Now:yyyy.MM.dd HH:mm:ss}").AlignCenter();
            });
        });

        OpenFolderDialog openFileDialog = new()
        {
            Title = "Оберіть шлях експорту PDF за результатами Клієнтів"
        };
        openFileDialog.ShowDialog();

        if (string.IsNullOrEmpty(openFileDialog.FolderName))
        {
            return;
        }

        pdfDocument.GeneratePdf(openFileDialog.FolderName + $"\\Клієнти - {DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf");

        MessageBoxResult result = MessageBox.Show(
            "Файл було успішно створено!\nЧи хочете ви відчинити папку де знаходиться файл?",
            "PDF Експорт - Клієнти",
            MessageBoxButton.YesNo,
            MessageBoxImage.Information
        );

        if (result == MessageBoxResult.Yes)
        {
            Process.Start("explorer.exe", openFileDialog.FolderName);
        }
    }

    private void MenuItemInstructorSpecializationExportPDF_Click(object sender, RoutedEventArgs e)
    {
        Document pdfDocument = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Header().Text("Список специалізацій інструкторів").FontSize(24).Bold().AlignCenter();
                page.Content().Column(col =>
                {
                    page.Margin(25);
                    foreach (SpecializationType specialization in mainWindowViewModel.SpecializationTypes)
                    {
                        col.Item().Text($"Спеціалізація: {specialization.Specialization}").FontSize(14);
                        col.Item().Text("");
                    }
                });
                page.Footer().Text($"Дата створення документу: {DateTime.Now:yyyy.MM.dd HH:mm:ss}").AlignCenter();
            });
        });

        OpenFolderDialog openFileDialog = new()
        {
            Title = "Оберіть шлях експорту PDF за результатами специалізацій інструкторів"
        };
        openFileDialog.ShowDialog();

        if (string.IsNullOrEmpty(openFileDialog.FolderName))
        {
            return;
        }

        pdfDocument.GeneratePdf(openFileDialog.FolderName + $"\\Спеціалізація Інструкторів - {DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf");

        MessageBoxResult result = MessageBox.Show(
            "Файл було успішно створено!\nЧи хочете ви відчинити папку де знаходиться файл?",
            "PDF Експорт - Cпециалізацій інструкторів",
            MessageBoxButton.YesNo,
            MessageBoxImage.Information
        );

        if (result == MessageBoxResult.Yes)
        {
            Process.Start("explorer.exe", openFileDialog.FolderName);
        }
    }

    private void MenuItemInstructorExportPDF_Click(object sender, RoutedEventArgs e)
    {
        Document pdfDocument = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Header().Text("Список інструкторів").FontSize(24).Bold().AlignCenter();
                page.Content().Column(col =>
                {
                    page.Margin(25);
                    foreach (Instructor instructor in mainWindowViewModel.Instructors)
                    {
                        col.Item().Text($"Ім'я: {instructor.FirstName}").FontSize(14);
                        col.Item().Text($"Прізвище: {instructor.LastName}").FontSize(14);
                        col.Item().Text($"Вік: {instructor.Age}").FontSize(14);
                        col.Item().Text($"Телефон: {instructor.PhoneNumber}").FontSize(14);
                        col.Item().Text($"Електронна пошта: {instructor.EmailAddress}").FontSize(14);
                        col.Item().Text($"Спеціалізація: {instructor.SpecializationName}").FontSize(14);
                        col.Item().Text("");
                    }
                });
                page.Footer().Text($"Дата створення документу: {DateTime.Now:yyyy.MM.dd HH:mm:ss}").AlignCenter();
            });
        });

        OpenFolderDialog openFileDialog = new()
        {
            Title = "Оберіть шлях експорту PDF за результатами Інструкторів"
        };
        openFileDialog.ShowDialog();

        if (string.IsNullOrEmpty(openFileDialog.FolderName))
        {
            return;
        }

        pdfDocument.GeneratePdf(openFileDialog.FolderName + $"\\Інструктори - {DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf");

        MessageBoxResult result = MessageBox.Show(
            "Файл було успішно створено!\nЧи хочете ви відчинити папку де знаходиться файл?",
            "PDF Експорт - Інструктори",
            MessageBoxButton.YesNo,
            MessageBoxImage.Information
        );

        if (result == MessageBoxResult.Yes)
        {
            Process.Start("explorer.exe", openFileDialog.FolderName);
        }

    }

    private void MenuItemTrainingExportPDF_Click(object sender, RoutedEventArgs e)
    {
        Document pdfDocument = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Header().Text("Список тренувань").FontSize(24).Bold().AlignCenter();
                page.Content().Column(col =>
                {
                    page.Margin(25);
                    foreach (Training training in mainWindowViewModel.Trainings)
                    {
                        col.Item().Text($"Дата: {training.Date:dd.MM.yyyy HH:mm}").FontSize(14);
                        col.Item().Text($"Тип тренування: {training.TrainingType}").FontSize(14);
                        col.Item().Text($"Басейн: {training.PoolName}").FontSize(14);
                        col.Item().Text($"Інструктор: {training.InstructorName}").FontSize(14);
                        col.Item().Text($"Клієнти: {training.ClientNames}").FontSize(14);
                        col.Item().Text("");
                    }
                });
                page.Footer().Text($"Дата створення документу: {DateTime.Now:yyyy.MM.dd HH:mm:ss}").AlignCenter();
            });
        });

        OpenFolderDialog openFileDialog = new()
        {
            Title = "Оберіть шлях експорту PDF за результатами Тренувань"
        };
        openFileDialog.ShowDialog();

        if (string.IsNullOrEmpty(openFileDialog.FolderName))
        {
            return;
        }

        pdfDocument.GeneratePdf(openFileDialog.FolderName + $"\\Тренування - {DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf");

        MessageBoxResult result = MessageBox.Show(
            "Файл було успішно створено!\nЧи хочете ви відчинити папку де знаходиться файл?",
            "PDF Експорт - Тренування",
            MessageBoxButton.YesNo,
            MessageBoxImage.Information
        );

        if (result == MessageBoxResult.Yes)
        {
            Process.Start("explorer.exe", openFileDialog.FolderName);
        }
    }

    private void MenuItemPoolExportPDF_Click(object sender, RoutedEventArgs e)
    {
        Document pdfDocument = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Header().Text("Список басейнів").FontSize(24).Bold().AlignCenter();
                page.Content().Column(col =>
                {
                    page.Margin(25);
                    foreach (Pool pool in mainWindowViewModel.Pools)
                    {
                        col.Item().Text($"Назва: {pool.Name}").FontSize(14);
                        col.Item().Text($"Кількість доріжок: {pool.LaneCount}").FontSize(14);
                        col.Item().Text($"Довжина: {pool.LengthAsString} м").FontSize(14);
                        col.Item().Text($"Глибина: {pool.DepthAsString} м").FontSize(14);
                        col.Item().Text($"Адреса: {pool.Address}").FontSize(14);
                        col.Item().Text("");
                    }
                });
                page.Footer().Text($"Дата створення документу: {DateTime.Now:yyyy.MM.dd HH:mm:ss}").AlignCenter();
            });
        });

        OpenFolderDialog openFileDialog = new()
        {
            Title = "Оберіть шлях експорту PDF за результатами Басейнів"
        };
        openFileDialog.ShowDialog();

        if (string.IsNullOrEmpty(openFileDialog.FolderName))
        {
            return;
        }
        
        pdfDocument.GeneratePdf(openFileDialog.FolderName + $"\\Басейни - {DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf");

        MessageBoxResult result = MessageBox.Show("Файл створено!",
            "Файл було успішно створено!\nЧи хочете ви відчинити папку де знаходиться файл?",
            MessageBoxButton.YesNo,
            MessageBoxImage.Information
        );

        if (result == MessageBoxResult.Yes)
        {
            Process.Start("explorer.exe", openFileDialog.FolderName);
        }
    }
}