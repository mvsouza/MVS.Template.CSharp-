using MVS.Template.CSharp.Application.Command;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MVS.Template.CSharp.UnitTest.Application.Command
{
    public class SolveCalculusCommandHandlerTests
    {
        [Fact]
        public async void Should_call_execute_solve()
        {
            var command = new SolveCalculusCommand("10+0");
            var handler = new SolveCalculusCommandHandler();
            Assert.Equal(10.0, await handler.Handle(command, default));
        }
    }
}
