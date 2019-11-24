using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using MVS.Template.CSharp.Infrastructure.Behaviors;
using Xunit;

namespace MVS.Template.CSharp.UnitTest.Infrastructure.Behavior
{
    public class LoggingBehaviorTests
    {
        public class LogFakeCommand : IRequest<int> { }
        [Fact]
        public async System.Threading.Tasks.Task Should_log_before_and_after_CommandHandling()
        {
            var logger = new Mock<ILogger<LoggingBehavior<LogFakeCommand, int>>>();
            var expectedResult = 1;
            var behavior = new LoggingBehavior<LogFakeCommand, int>(logger.Object);
            var result = await behavior.Handle(new LogFakeCommand(), default(CancellationToken), () => Task.FromResult(expectedResult));
            logger.VerifyLogHasMessage("Handling LogFakeCommand");
            logger.VerifyLogHasMessage("Handled LogFakeCommand");
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async System.Threading.Tasks.Task Should_log_before_and_after_CommandHandling_Async()
        {
            var logger = new Mock<ILogger<LoggingBehavior<LogFakeCommand, int>>>();
            var expectedResult = 1;
            var behavior = new LoggingBehavior<LogFakeCommand, int>(logger.Object);
            var result = await behavior.Handle(new LogFakeCommand(), default(CancellationToken), async () => {
                await Task.Delay(1);
                return expectedResult;
            });
            logger.VerifyLogHasMessage("Handling LogFakeCommand");
            logger.VerifyLogHasMessage("Handled LogFakeCommand");
            Assert.Equal(expectedResult, result);
        }
    }

    public static class ExtedLogger
    {

        public static void VerifyLogHasMessage<TLogged>(this Mock<ILogger<TLogged>> logger, string message)
        {
            logger.Verify(l => l.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<FormattedLogValues>(f => f.ToString().Contains(message)), null, It.IsAny<Func<Object, Exception, string>>()), Times.Once());
        }
    }
}
