using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMLBS.Utils
{
    internal static class ExpressionParametersReplacer
    {
        internal static void ReplaceEvalParameters(List<double> parameters,
                                                  List<Token> functionExpression)
        {
            for (int i = 0; i < functionExpression.Count; i++)
            {
                Token token = functionExpression[i];
                if (token.Type == TokenTypeEnum.NUMBER && token.Value is string)
                {
                    int paramPosition = Convert.ToInt32(
                        (token.Value as string)[1..]
                    );

                    Token newToken = new(TokenTypeEnum.NUMBER, parameters[paramPosition - 1]);
                    functionExpression[i] = newToken;
                }
            }
        }
    }
}
