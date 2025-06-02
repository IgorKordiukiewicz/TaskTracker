namespace Analytics.Infrastructure.Models;

public interface IProjection
{
    int Id { get; set; }
    Guid ProjectId { get; set; }
}
