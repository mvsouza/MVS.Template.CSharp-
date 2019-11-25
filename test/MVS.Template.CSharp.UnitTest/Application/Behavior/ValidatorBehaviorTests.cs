using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using MVS.Template.CSharp.Application.Behavior;
using Xunit;

namespace MVS.Template.CSharp.UnitTest.Application.Behavior
{
    public class ValidatorBehaviorTests
    {
        private Mock<IValidator<object>> _validator;
        private IValidator<object>[] _validators;
        private bool _delegateHaveBeenCalled;

        public ValidatorBehaviorTests()
        {

            _validator = new Mock<IValidator<object>>();
            _validators = new IValidator<object>[1];
            _delegateHaveBeenCalled = false;

        }


        [Fact]
        public async Task Should_run_next_when_there_is_no_validatior_Async()
        {
            _validators = new IValidator<object>[0];
            var behavior = new ValidatorBehavior<object, object>(_validators);
            await behavior.Handle(new object(), default(CancellationToken), async () => {
                await Task.Delay(1);
                _delegateHaveBeenCalled = true;
                return new Object();
            });
            Assert.True(_delegateHaveBeenCalled);
        }

        [Fact]
        public async Task Should_run_next_when_there_is_no_validatior()
        {
            _validators = new IValidator<object>[0];
            var behavior = new ValidatorBehavior<object, object>(_validators);
            await behavior.Handle(new object(), default(CancellationToken), NextHandle);
            Assert.True(_delegateHaveBeenCalled);
        }

        [Fact]
        public async Task Should_run_next_when_everything_is_valid()
        {
            var validationResult = new ValidationResult(new List<ValidationFailure>());
            _validator.Setup(v => v.Validate(It.IsAny<object>()))
                     .Returns(validationResult);

            _validators[0] = _validator.Object;

            var behavior = new ValidatorBehavior<object, object>(_validators);
            await behavior.Handle(new object(), default(CancellationToken), NextHandle);
            Assert.True(_delegateHaveBeenCalled);
        }
        [Fact]
        public void Should_throw_an_exception_when_there_is_a_ValidatiorFailure()
        {
            var failures = new List<ValidationFailure>() { new ValidationFailure("teste", "teste") };
            var validationResult = new ValidationResult(failures);
            _validator.Setup(v => v.Validate(It.IsAny<object>()))
                     .Returns(validationResult);

            _validators[0] = _validator.Object;

            var behavior = new ValidatorBehavior<object, object>(_validators);
            Assert.False(_delegateHaveBeenCalled);
            Assert.ThrowsAsync<ValidationException>(async () => {
                await behavior.Handle(new object(), default(CancellationToken), NextHandle);
            });

        }
        public Task<object> NextHandle()
        {
            _delegateHaveBeenCalled = true;
            return Task.FromResult(new Object());
        }
    }
}
