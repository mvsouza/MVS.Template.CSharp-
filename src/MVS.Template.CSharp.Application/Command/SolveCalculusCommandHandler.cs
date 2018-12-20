using MediatR;
using MVS.Template.CSharp.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace MVS.Template.CSharp.Application.Command
{
    public class SolveCalculusCommandHandler: IRequestHandler<SolveCalculusCommand, double>
    {
        public async Task<double> Handle(SolveCalculusCommand command, CancellationToken cancellationToken)
        {
            Factor f = new Factor(command.Calculus);

            return f.Solve();
        }
    }
}