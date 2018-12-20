using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVS.Template.CSharp.Domain.Entities
{
    public class Factor
    {
        public string Calculus { get; set; }
        public Factor(string calculus)
        {
            Calculus = calculus;
        }
        public double Solve()
        {
            return 10.0;
        }
    }
}
