using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMLBS.Utils
{
    internal static class AMLBSstd
    {

        internal static void UseStd(this AMLBS amlsb)
        {
            amlsb.DefineNativeOperator("+", 1, (l, r) => l + r);
            amlsb.DefineNativeOperator("-", 1, (l, r) => l - r);
            amlsb.DefineNativeOperator("*", 2, (l, r) => l * r);
            amlsb.DefineNativeOperator("/", 2, (l, r) => l / r);
            amlsb.DefineNativeOperator("^", 3, (l, r) => Math.Pow(l, r));

            amlsb.DefineNativeOperator("==", 3, (l, r) => Convert.ToInt32(l == r));
            amlsb.DefineNativeOperator("!=", 3, (l, r) => Convert.ToInt32(l != r));

            amlsb.DefineNativeOperator(">", 3, (l, r) => Convert.ToInt32(l > r));
            amlsb.DefineNativeOperator(">=", 3, (l, r) => Convert.ToInt32(l >= r));

            amlsb.DefineNativeOperator("<", 3, (l, r) => Convert.ToInt32(l < r));
            amlsb.DefineNativeOperator("<=", 3, (l, r) => Convert.ToInt32(l <= r));

            amlsb.DefineNativeOperator("&&", 2, (l, r) =>
            {
                var boolL = Convert.ToBoolean(l);
                var boolR = Convert.ToBoolean(r);

                return Convert.ToInt32(boolL && boolR);
            });

            amlsb.DefineNativeOperator("||", 1, (l, r) =>
            {
                var boolL = Convert.ToBoolean(l);
                var boolR = Convert.ToBoolean(r);

                return Convert.ToInt32(boolL || boolR);
            });

            amlsb.DefineNativeFunction("sqrt", 1, (parameters) => Math.Sqrt(parameters[0]));
        }

    }
}
