using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using MVS.Template.CSharp.Application.Command;

namespace MVS.Template.CSharp.Application.Validation
{
    public class SolveCalculusCommandValidation : AbstractValidator<SolveCalculusCommand>
    {
        public SolveCalculusCommandValidation()
        {
            RuleFor(command => command.Calculus).NotEmpty().WithMessage("\"Calculus\" field should'nt be empty.");
        }
    }
}
