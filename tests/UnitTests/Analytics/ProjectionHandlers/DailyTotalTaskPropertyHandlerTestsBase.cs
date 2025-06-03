using Analytics.Infrastructure.Models;
using Analytics.ProjectionHandlers;
using Analytics.Services;
using Domain.Common;
using Domain.Events;
using UnitTests.Analytics.Helpers;

namespace UnitTests.Analytics.ProjectionHandlers;

public abstract class DailyTotalTaskPropertyHandlerTestsBase<TProjection, TProperty, THandler>
    where TProjection : IDailyCountProjection
    where THandler : IProjectionHandler
{
    protected readonly DateTime _currentDay = new(2025, 05, 31, 12, 0, 0);
    protected readonly DateTime _previousDay = new(2025, 05, 30, 12, 0, 0);
    protected readonly Guid _projectId = Guid.NewGuid();
    protected readonly Func<TProjection, TProperty, bool> _predicate;
    protected readonly Func<TestRepository, THandler> _handlerCreateFunc;
    protected readonly TProperty _property1;
    protected readonly TProperty _property2;

    protected DailyTotalTaskPropertyHandlerTestsBase(Func<TProjection, TProperty, bool> predicate, Func<TestRepository, THandler> handlerCreateFunc, TProperty oldProperty, TProperty newProperty)
    {
        _predicate = predicate;
        _handlerCreateFunc = handlerCreateFunc;
        _property1 = oldProperty;
        _property2 = newProperty;
    }

    protected async Task<(THandler Sut, TestRepository Repository)> Arrange(params IProjection[] projections)
    {
        var repository = new TestRepository
        {
            Projections = [.. projections]
        };
        var sut = _handlerCreateFunc(repository);
        await sut.InitializeState(_projectId);

        return (sut, repository);
    }

    protected TProjection? GetProjection(TestRepository repository, TProperty property, DateTime now)
    {
        return repository.Projections
            .OfType<TProjection>()
            .FirstOrDefault(x => x.ProjectId == _projectId && x.Date == now.Date && _predicate(x, property));
    }

    protected abstract TProjection CreateProjection(TProperty property, DateTime date, int count);
}
