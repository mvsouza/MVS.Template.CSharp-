using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using LightBDD.XUnit2;
using MVS.Template.CSharp.FunctionalTest.Setup;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Xunit;
using static MVS.Template.CSharp.FunctionalTest.Setup.MathSolverScenarioBase;

namespace MVS.Template.CSharp.FunctionalTest.Features
{
    public partial class Solve_Calculus : FeatureFixture
    {
        private string _calculus;
        private IConfigurationRoot _configuration;
        private HttpResponseMessage _response;

        public Solve_Calculus()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());
            _configuration = builder.Build();
        }

        private void Given_the_calculus(string calculus)
        {
            _calculus = calculus;
        }


        private async void When_I_request_a_calculus_to_be_solved()
        {

            string json_requestEncouter = JsonConvert.SerializeObject($"\"{_calculus}\"");
            var content = new StringContent(json_requestEncouter, Encoding.UTF8, "application/json");
            var scenarioBase = new MathSolverScenarioBase();
            _response = await scenarioBase.CreateServer().CreateClient()
                .PostAsync(Post.Solve(), content);
        }

        private void Then_should_result_the_value(double value)
        {
            _response.EnsureSuccessStatusCode();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(_response.Content.ReadAsStringAsync().Result);
            Assert.True(value == (double)result);
        }
    }
}
