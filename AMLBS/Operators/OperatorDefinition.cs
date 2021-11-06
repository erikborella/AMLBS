using AMLBS.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMLBS.Operators
{

    public delegate double NativeOperatorCall(double l, double r);

    internal record BaseOperatorDefinition(int Precedence);
        
    internal record OperatorDefinition(List<Token> Tokens, int Precedence) 
        : BaseOperatorDefinition(Precedence);
    internal record NativeOperatorDefinition(int Precedence, NativeOperatorCall NativeCall) 
        : BaseOperatorDefinition(Precedence);
}
