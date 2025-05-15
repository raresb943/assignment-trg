using Dispatcher.Core.Interfaces;
using Dispatcher.Core.Models;
using Dispatcher.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dispatcher.Tests.Services;

public class BrowsingServiceTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly Mock<INodeCommunicationService> _mockNodeCommunication;
    private readonly Mock<ILogger<BrowsingService>> _mockLogger;
    private readonly BrowsingService _service;

    public BrowsingServiceTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _mockNodeCommunication = new Mock<INodeCommunicationService>();
        _mockLogger = new Mock<ILogger<BrowsingService>>();

        _service = new BrowsingService(
            _mockRepository.Object,
            _mockNodeCommunication.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task ProcessBrowsingRequestAsync_ReturnsValidResponse_WhenNodeServiceRespondsSuccessfully()
    {
        // Arrange
        var request = new BrowsingRequest { Url = "https://example.com" };
        var taskId = Guid.NewGuid();
        var htmlContent = "<html><body>Example content</body></html>";

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<BrowsingTask>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BrowsingTask task, CancellationToken _) =>
            {
                task.Id = taskId;
                return task;
            });

        _mockNodeCommunication.Setup(n => n.SendBrowsingTaskToNodeAsync(It.IsAny<BrowsingTask>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(htmlContent);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<BrowsingTask>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BrowsingTask task, CancellationToken _) => task);

        // Act
        var result = await _service.ProcessBrowsingRequestAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(taskId, result.TaskId);
        Assert.Equal(htmlContent, result.HtmlContent);
        Assert.NotEqual(default, result.CompletedAt);

        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<BrowsingTask>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockNodeCommunication.Verify(n => n.SendBrowsingTaskToNodeAsync(It.IsAny<BrowsingTask>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<BrowsingTask>(t => t.Status == BrowsingTaskStatus.Completed), It.IsAny<CancellationToken>()), Times.Once);
    }
}
