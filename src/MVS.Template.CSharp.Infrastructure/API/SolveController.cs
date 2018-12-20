using MVS.Template.CSharp.Application.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace MVS.Template.CSharp.Infrastructure.API
{
    [Produces("application/json")]
    [Route("api/v1/Solve")]
    public class  SolveController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SolveController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Post([FromBody]string calculus)
        {
            var command = new SolveCalculusCommand(calculus);
            var result = await _mediator.Send(command);
            return (IActionResult)Ok(result);
        }
    }
}
