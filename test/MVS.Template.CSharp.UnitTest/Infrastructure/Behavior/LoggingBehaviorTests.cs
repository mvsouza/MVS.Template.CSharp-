using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using MVS.Template.CSharp.Infrastructure.Behaviors;
using Xunit;

namespace MVS.Template.CSharp.UnitTest.Infrastructure.Behavior
{
    public class LoggingBehaviorTests
    {
        private Mock<ILogger<LoggingBehavior<LogFakeCommand, int>>> _logger;
        private LoggingBehavior<LogFakeCommand, int> _behavior;

        public class LogFakeCommand : IRequest<int> { }

        public LoggingBehaviorTests()
        {
            _logger = new Mock<ILogger<LoggingBehavior<LogFakeCommand, int>>>();
            _behavior = new LoggingBehavior<LogFakeCommand, int>(_logger.Object);
        }
        public async Task Should_log_before_and_after_CommandHandling(Func<int, RequestHandlerDelegate<int>> solveResult)
        {
            
            var expectedResult = 1;
            var result = await _behavior.Handle(new LogFakeCommand(), default(CancellationToken), solveResult(expectedResult));
            _logger.VerifyLogHasMessage("Handling LogFakeCommand");
            _logger.VerifyLogHasMessage("Handled LogFakeCommand");
            Assert.Equal(expectedResult, result);
        }
        [Fact]
        public async Task Should_log_before_and_after_CommandHandling_Sync()
        {
            await Should_log_before_and_after_CommandHandling((expectedResult) => () => Task.FromResult(expectedResult));
        }
        
        [Fact]
        public async Task Should_log_before_and_after_CommandHandling_Async()
        {
            await Should_log_before_and_after_CommandHandling((expectedResult) => async () => {
                await Task.Delay(1);
                return expectedResult;
            });
        }
    }

    public static class ExtedLogger
    {

        public static void VerifyLogHasMessage<TLogged>(this Mock<ILogger<TLogged>> logger, string message)
        {
            logger.Verify(l => l.Log(LogLevel.Information, It.IsAny<EventId>(), 
                It.Is<It.IsAnyType>((object v, Type _) => v.ToString().Contains(message)), null,
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once());
        }
    }
}
