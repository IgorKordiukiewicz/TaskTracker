namespace Analytics.Infrastructure.Models;

public interface IProjection
{
    int Id { get; set; }
    Guid ProjectId { get; set; }
}

public interface IDailyCountProjection : IProjection
{
    DateTime Date { get; set; }
    int Count { get; set; }
}