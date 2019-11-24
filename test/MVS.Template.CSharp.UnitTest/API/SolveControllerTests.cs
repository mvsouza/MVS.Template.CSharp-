using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MVS.Template.CSharp.Infrastructure.API;
using Xunit;

namespace MVS.Template.CSharp.UnitTest.API
{
    public class SolveControllerTests
    {
        [Fact]
        public async Task Should_send_SolveCommand()
        {
            var solvedValue = 10.0;
            var mediatr = new Mock<IMediator>();
            mediatr.Setup(m => m.Send<double>(It.IsAny<IRequest<double>>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(solvedValue));
            var controller = new SolveController(mediatr.Object);
            var result = await controller.Post("(10*1)");
            Assert.Equal(solvedValue, Assert.IsType<OkObjectResult>(result).Value);

        }
        [Fact]
        public async Task Should_send_SolveCommand_Async()
        {
            var solvedValue = 10.0;
            var mediatr = new Mock<IMediator>();
            var tcs = new TaskCompletionSource<double>();
            
            mediatr.Setup(m => m.Send<double>(It.IsAny<IRequest<double>>(), It.IsAny<CancellationToken>())).Returns(async () => {
                await Task.Delay(1);
                return solvedValue;
            });
            tcs.TrySetResult(solvedValue);
            var controller = new SolveController(mediatr.Object);
            var result = await controller.Post("(10*1)");
            Assert.Equal(solvedValue, Assert.IsType<OkObjectResult>(result).Value);

        }
    }
}
