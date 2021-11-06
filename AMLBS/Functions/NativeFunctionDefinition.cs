using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMLBS.Functions
{
    public delegate double NativeCall(params double[] parameters);

    internal record NativeFunctionDefinition(int ParametersCount, NativeCall NativeCall);
}
