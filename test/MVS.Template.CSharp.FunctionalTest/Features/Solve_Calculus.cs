using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.XUnit2;

namespace MVS.Template.CSharp.FunctionalTest.Features
{
    [FeatureDescription(
@"In order validate a fatorial calculus result
As a Student
I want to request its solution.")]
    [Label("Story-1")]
    public partial class Solve_Calculus
    {
        [Scenario]
        [MultiAssert]
        public async void Solve_a_add()
        {
            await Runner.RunScenarioActionsAsync(
                        _ => Given_the_calculus("5+5"),
                        _ => When_I_request_a_calculus_to_be_solved(),
                        _ => Then_should_result_the_value(10)
                    );
        }
    }
}
