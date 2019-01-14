using System;
using System.Collections.Generic;
using System.Text;
using MVS.Template.CSharp.Application.Command;
using MVS.Template.CSharp.Application.Validation;
using Xunit;

namespace MVS.Template.CSharp.UnitTest.Application.Validation
{
    public class SolveCalculusCommandValidationTests
    {
        [Fact]
        public void Should_return_invalid_when_calculus_is_empty()
        {
            var validation = new SolveCalculusCommandValidation();
            var validationObj = new SolveCalculusCommand("");
            var result = validation.Validate(validationObj);
            Assert.False(result.IsValid);
        }

    }
}
