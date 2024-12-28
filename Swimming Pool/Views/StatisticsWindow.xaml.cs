using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Swimming_Pool.Views;

public partial class StatisticsWindow : Window
{
    public StatisticsWindow()
    {
        InitializeComponent();
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TabItem selectedTab = (TabItem)((TabControl)sender).SelectedItem;
        switch (selectedTab.Name)
        {
            case "InstructorEngagementTab":
                CreateInstructorEngagementPlot();
                break;
            case "InstructorClientsTab":
                CreateInstructorClientsPlot();
                break;
            case "TrainingsTab":
                CreateTrainingsPlot();
                break;
            case "SubscriptionTab":
                CreateSubscriptionPlot();
                break;
            case "SpecializationTab":
                CreateSpecializationPlot();
                break;
        }

    }

    private async void CreateInstructorEngagementPlot()
    {
        InstructorEngagementPanel.Children.Clear();
        InstructorEngagementPanel.ColumnDefinitions.Clear();
        InstructorEngagementPanel.RowDefinitions.Clear();
        var data = await Database.GetInstructorEngagement();
        data.Sort((a, b) => b.Count.CompareTo(a.Count));
        int max = 0;
        foreach (var item in data)
        {
            if (item.Count > max)
            {
                max = item.Count;
            }
            InstructorEngagementPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
        }
        InstructorEngagementPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
        InstructorEngagementPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
        InstructorEngagementPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(4, GridUnitType.Star) });
        InstructorEngagementPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
        InstructorEngagementPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

        var labelV = new Label()
        {
            Content = "Number of Clients",
            FontWeight = FontWeights.Bold,
            LayoutTransform = new RotateTransform(-90),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        InstructorEngagementPanel.Children.Add(labelV);
        Grid.SetColumn(labelV, 0);
        Grid.SetRow(labelV, 0);
        Grid.SetRowSpan(labelV, 3);

        var labelH = new Label()
        {
            Content = "Instructors",
            FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom
        };
        InstructorEngagementPanel.Children.Add(labelH);
        Grid.SetColumn(labelH, 0);
        Grid.SetRow(labelH, 2);
        Grid.SetColumnSpan(labelH, data.Count + 1);

        for (int i = 0; i < data.Count; i++)
        {
            TextBlock num = new()
            {
                Text = data[i].Count.ToString(),
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            InstructorEngagementPanel.Children.Add(num);
            Grid.SetColumn(num, i + 1);
            Grid.SetRow(num, 0);
            var bar = new ProgressBar
            {
                Value = data[i].Count,
                Maximum = max,
                Width = 40,
                Margin = new Thickness(5),
                Orientation = Orientation.Vertical
            };
            InstructorEngagementPanel.Children.Add(bar);
            Grid.SetColumn(bar, i + 1);
            Grid.SetRow(bar, 1);
            TextBlock tb = new()
            {
                Text = data[i].Name,
                Margin = new Thickness(5),
                //                LayoutTransform = new RotateTransform(90)
            };
            InstructorEngagementPanel.Children.Add(tb);
            Grid.SetColumn(tb, i + 1);
            Grid.SetRow(tb, 2);
        }
    }

    private async void CreateInstructorClientsPlot()
    {
        InstructorClientsPanel.Children.Clear();
        InstructorClientsPanel.ColumnDefinitions.Clear();
        InstructorClientsPanel.RowDefinitions.Clear();
        var data = await Database.GetInstructorClients();
        data.Sort((a, b) => b.Count.CompareTo(a.Count));
        int max = 0;
        foreach(var item in data)
        {
            if (item.Count > max)
            {
                max = item.Count;
            }
            InstructorClientsPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
        }
        InstructorClientsPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto)});
        InstructorClientsPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto)});
        InstructorClientsPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(4, GridUnitType.Star)});
        InstructorClientsPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star)});
        InstructorClientsPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto)});

        var labelV = new Label()
        {
            Content = "Number of Clients",
            FontWeight = FontWeights.Bold,
            LayoutTransform = new RotateTransform(-90),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        InstructorClientsPanel.Children.Add(labelV);
        Grid.SetColumn(labelV, 0);
        Grid.SetRow(labelV, 0);
        Grid.SetRowSpan(labelV, 3);

        var labelH = new Label()
        {
            Content = "Instructors",
            FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom
        };
        InstructorClientsPanel.Children.Add(labelH);
        Grid.SetColumn(labelH, 0);
        Grid.SetRow(labelH, 2);
        Grid.SetColumnSpan(labelH, data.Count + 1);

        for (int i = 0; i < data.Count; i++)
        {
            TextBlock num = new()
            {
                Text = data[i].Count.ToString(),
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            InstructorClientsPanel.Children.Add(num);
            Grid.SetColumn(num, i + 1);
            Grid.SetRow(num, 0);
            var bar = new ProgressBar
            {
                Value = data[i].Count,
                Maximum = max,
                Width = 40,
                Margin = new Thickness(5),
                Orientation = Orientation.Vertical
            };
            InstructorClientsPanel.Children.Add(bar);
            Grid.SetColumn(bar, i + 1);
            Grid.SetRow(bar, 1);
            TextBlock tb = new()
            {
                Text = data[i].Name,
                Margin = new Thickness(5),
            };
            InstructorClientsPanel.Children.Add(tb);
            Grid.SetColumn(tb, i + 1);
            Grid.SetRow(tb, 2);
        }
    }

    private async void CreateTrainingsPlot()
    {
        TrainingPanel.Children.Clear();
        TrainingPanel.ColumnDefinitions.Clear();
        TrainingPanel.RowDefinitions.Clear();
        var data = await Database.GetTrainingsStatistics();
        data.Sort((a, b) => a.Date.CompareTo(b.Date));
        float max = 0;
        foreach(var item in data)
        {
            if (item.Price > max)
            {
                max = item.Price;
            }
            TrainingPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
        }
        TrainingPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto)});
        TrainingPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto)});
        TrainingPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto)});
        TrainingPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(4, GridUnitType.Star)});
        TrainingPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star)});
        TrainingPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto)});

        var labelV = new Label()
        {
            Content = "Number of Clients and total money from Subscriptions",
            FontWeight = FontWeights.Bold,
            LayoutTransform = new RotateTransform(-90),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        TrainingPanel.Children.Add(labelV);
        Grid.SetColumn(labelV, 0);
        Grid.SetRow(labelV, 0);
        Grid.SetRowSpan(labelV, 4);

        var labelH = new Label()
        {
            Content = "Dates",
            FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom
        };
        TrainingPanel.Children.Add(labelH);
        Grid.SetColumn(labelH, 0);
        Grid.SetRow(labelH, 4);
        Grid.SetColumnSpan(labelH, data.Count + 1);

        for (int i = 0; i < data.Count; i++)
        {
            TextBlock sum = new()
            {
                Text = $"${data[i].Price} amount",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            TrainingPanel.Children.Add(sum);
            Grid.SetColumn(sum, i + 1);
            Grid.SetRow(sum, 0);
            TextBlock num = new()
            {
                Text = $"{data[i].Clients} clients",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            TrainingPanel.Children.Add(num);
            Grid.SetColumn(num, i + 1);
            Grid.SetRow(num, 1);
            var bar = new ProgressBar
            {
                Value = data[i].Price,
                Maximum = max,
                Width = 40,
                Margin = new Thickness(5),
                Orientation = Orientation.Vertical
            };
            TrainingPanel.Children.Add(bar);
            Grid.SetColumn(bar, i + 1);
            Grid.SetRow(bar, 2);
            TextBlock tb = new()
            {
                Text = data[i].Date.ToString("yyyy/MM/dd"),
                Margin = new Thickness(5),
            };
            TrainingPanel.Children.Add(tb);
            Grid.SetColumn(tb, i + 1);
            Grid.SetRow(tb, 3);
        }
    }

    private async void CreateSubscriptionPlot()
    {
        SubscriptionPanel.Children.Clear();
        SubscriptionPanel.ColumnDefinitions.Clear();
        SubscriptionPanel.RowDefinitions.Clear();
        var data = await Database.GetSubscriptionStatistics();
        data.Sort((a, b) => b.Price.CompareTo(a.Price));
        float max = 0;
        foreach(var item in data)
        {
            if (item.Price > max)
            {
                max = item.Price;
            }
            SubscriptionPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
        }
        SubscriptionPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto)});
        SubscriptionPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto)});
        SubscriptionPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto)});
        SubscriptionPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(4, GridUnitType.Star)});
        SubscriptionPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star)});
        SubscriptionPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto)});

        var labelV = new Label()
        {
            Content = "Number of Clients and average price for Subscription Types",
            FontWeight = FontWeights.Bold,
            LayoutTransform = new RotateTransform(-90),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        SubscriptionPanel.Children.Add(labelV);
        Grid.SetColumn(labelV, 0);
        Grid.SetRow(labelV, 0);
        Grid.SetRowSpan(labelV, 4);

        var labelH = new Label()
        {
            Content = "Subscription Types",
            FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom
        };
        SubscriptionPanel.Children.Add(labelH);
        Grid.SetColumn(labelH, 0);
        Grid.SetRow(labelH, 4);
        Grid.SetColumnSpan(labelH, data.Count + 1);

        for (int i = 0; i < data.Count; i++)
        {
            TextBlock sum = new()
            {
                Text = $"${data[i].Price} amount",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            SubscriptionPanel.Children.Add(sum);
            Grid.SetColumn(sum, i + 1);
            Grid.SetRow(sum, 0);
            TextBlock num = new()
            {
                Text = $"{data[i].Clients} clients",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            SubscriptionPanel.Children.Add(num);
            Grid.SetColumn(num, i + 1);
            Grid.SetRow(num, 1);
            var bar = new ProgressBar
            {
                Value = data[i].Price,
                Maximum = max,
                Width = 40,
                Margin = new Thickness(5),
                Orientation = Orientation.Vertical
            };
            SubscriptionPanel.Children.Add(bar);
            Grid.SetColumn(bar, i + 1);
            Grid.SetRow(bar, 2);
            TextBlock tb = new()
            {
                Text = data[i].SubscriptionType,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(5)
            };
            SubscriptionPanel.Children.Add(tb);
            Grid.SetColumn(tb, i + 1);
            Grid.SetRow(tb, 3);
        }
    }

    private async void CreateSpecializationPlot()
    {
        SpecializationPanel.Children.Clear();
        SpecializationPanel.ColumnDefinitions.Clear();
        SpecializationPanel.RowDefinitions.Clear();
        var data = await Database.GetAverageAgePerSpecializationStatistics();
        data.Sort((a, b) => b.Age.CompareTo(a.Age));
        float max = 0;
        foreach (var item in data)
        {
            if (item.Age > max)
            {
                max = item.Age;
            }
            SpecializationPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
        }
        SpecializationPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
        SpecializationPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
        SpecializationPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
        SpecializationPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(4, GridUnitType.Star) });
        SpecializationPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
        SpecializationPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

        var labelV = new Label()
        {
            Content = "Number of Clients and average age for Instructor Specialization",
            FontWeight = FontWeights.Bold,
            LayoutTransform = new RotateTransform(-90),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        SpecializationPanel.Children.Add(labelV);
        Grid.SetColumn(labelV, 0);
        Grid.SetRow(labelV, 0);
        Grid.SetRowSpan(labelV, 4);

        var labelH = new Label()
        {
            Content = "Specialization Types",
            FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom
        };
        SpecializationPanel.Children.Add(labelH);
        Grid.SetColumn(labelH, 0);
        Grid.SetRow(labelH, 4);
        Grid.SetColumnSpan(labelH, data.Count + 1);

        for (int i = 0; i < data.Count; i++)
        {
            TextBlock sum = new()
            {
                Text = $"{data[i].Age} age",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            SpecializationPanel.Children.Add(sum);
            Grid.SetColumn(sum, i + 1);
            Grid.SetRow(sum, 0);
            TextBlock num = new()
            {
                Text = $"{data[i].Clients} clients",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            SpecializationPanel.Children.Add(num);
            Grid.SetColumn(num, i + 1);
            Grid.SetRow(num, 1);
            var bar = new ProgressBar
            {
                Value = data[i].Age,
                Maximum = max,
                Width = 40,
                Margin = new Thickness(5),
                Orientation = Orientation.Vertical
            };
            SpecializationPanel.Children.Add(bar);
            Grid.SetColumn(bar, i + 1);
            Grid.SetRow(bar, 2);
            TextBlock tb = new()
            {
                Text = data[i].Specialization,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(5)
            };
            SpecializationPanel.Children.Add(tb);
            Grid.SetColumn(tb, i + 1);
            Grid.SetRow(tb, 3);
        }
    }
}