using Dispatcher.Core.Models;

namespace Dispatcher.Core.Interfaces;

public interface ITaskRepository
{
    Task<BrowsingTask> CreateAsync(BrowsingTask task, CancellationToken cancellationToken = default);
    Task<BrowsingTask?> GetByIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<BrowsingTask> UpdateAsync(BrowsingTask task, CancellationToken cancellationToken = default);
    Task<IEnumerable<BrowsingTask>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BrowsingTask?> GetTaskById(Guid taskId);
    Task UpdateTask(BrowsingTask task);
}
