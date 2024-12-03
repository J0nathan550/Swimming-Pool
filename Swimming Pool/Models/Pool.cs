using CommunityToolkit.Mvvm.Input;
using Swimming_Pool.Views;

namespace Swimming_Pool.Models;

public partial class Pool
{
    public int PoolId { get; set; }
    public string? Name { get; set; }
    public int LaneCount { get; set; }
    public float Length { get; set; }
    public string LengthAsString { get => Length.ToString("F2"); }
    public float Depth { get; set; }
    public string DepthAsString { get => Depth.ToString("F2"); }
    public string? Address { get; set; }


    [RelayCommand]
    private static void UpdatePool(int poolId)
    {
        UpdatePoolWindow updateSubscriptionWindow = new()
        {
            Owner = MainWindow.MainWindowInstance
        };
        updateSubscriptionWindow.Initialize(poolId);
        updateSubscriptionWindow.ShowDialog();
    }
}