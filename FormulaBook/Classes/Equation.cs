using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaBook.Classes
{
    public class Equation
    {
        private string equation;
        private string unknown;
        public Equation(string equation, string unknownVariable)
        {
            this.equation = equation;
            unknown = unknownVariable;
        }
        public double Solve()
        {
            return -1;
        }
    }
}
