using System;
using System.Collections.Generic;
using System.Text;
using MVS.Template.CSharp.Domain.Entities;
using Xunit;

namespace MVS.Template.CSharp.UnitTest.Domain
{
    public class FactorTests
    {
        [Fact]
        public void Single_factor_solution()
        {
            var factor = new Factor("10");
            Assert.Equal(10, factor.Solve());
        }
    }
}
