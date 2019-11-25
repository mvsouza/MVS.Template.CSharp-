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
        public async Task Should_send_SolveCommand_Sync()
        {
            await Should_send_SolveCommand((solvedValue) => () => Task.FromResult(solvedValue));
        }
        [Fact]
        public async Task Should_send_SolveCommand_Async()
        {
            await Should_send_SolveCommand((solvedValue) => (async () => {
                await Task.Delay(1);
                return solvedValue;
            }));
        }
        public async Task Should_send_SolveCommand(Func<double,Func<Task<double>>> solveResult)
        {
            var solvedValue = 10.0;
            var mediatr = new Mock<IMediator>();
            mediatr.Setup(m => m.Send<double>(It.IsAny<IRequest<double>>(), It.IsAny<CancellationToken>())).Returns(solveResult(solvedValue));
            var controller = new SolveController(mediatr.Object);
            var result = await controller.Post("(10*1)");
            Assert.Equal(solvedValue, Assert.IsType<OkObjectResult>(result).Value);

        }
    }
}
