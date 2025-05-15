using Dispatcher.Core.Interfaces;
using Dispatcher.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Dispatcher.Core.Services;

public class BrowsingTaskRepository : ITaskRepository
{
    private readonly AppDbContext _dbContext;

    public BrowsingTaskRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BrowsingTask> CreateAsync(BrowsingTask task, CancellationToken cancellationToken = default)
    {
        await _dbContext.BrowsingTasks.AddAsync(task, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return task;
    }

    public async Task<BrowsingTask?> GetByIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.BrowsingTasks
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);
    }

    public async Task<BrowsingTask> UpdateAsync(BrowsingTask task, CancellationToken cancellationToken = default)
    {
        _dbContext.BrowsingTasks.Update(task);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return task;
    }
    public async Task<IEnumerable<BrowsingTask>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.BrowsingTasks.ToListAsync(cancellationToken);
    }

    public async Task<BrowsingTask?> GetTaskById(Guid taskId)
    {
        return await _dbContext.BrowsingTasks
            .FirstOrDefaultAsync(t => t.Id == taskId);
    }

    public async Task UpdateTask(BrowsingTask task)
    {
        _dbContext.BrowsingTasks.Update(task);
        await _dbContext.SaveChangesAsync();
    }
}
