namespace Swimming_Pool_One_Lab.Models;

public class Training
{
    public int TrainingId { get; set; }
    public DateTime Date { get; set; }
    public string? TrainingType { get; set; }
    public string? PoolName { get; set; }
    public int ClientId { get; set; }
    public string? ClientName { get; set; }
    public int InstructorId { get; set; }
    public string? InstructorName { get; set; }
}