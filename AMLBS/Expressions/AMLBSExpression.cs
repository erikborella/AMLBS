using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMLBS.Expressions
{
    internal static class AMLBSExpression
    { 
        internal static double? EvalExpression(List<Token> tokens, AMLBS amlbs)
        {
            if (tokens.Count == 0)
                return null;

            tokens = EvalNegations(new Queue<Token>(tokens));
            tokens = RPNConversor.Convert(tokens, amlbs);

            double result = StackMachine.Evaluate(tokens, amlbs);

            return result;
        
        }

        private static List<Token> EvalNegations(Queue<Token> tokens)
        {
            List<Token> resultTokens = new();
            bool isExpressionStart = true;

            while (tokens.Count != 0)
            {
                var currentToken = tokens.Dequeue();
                Token newToken;

                if (currentToken.Type == TokenTypeEnum.SYMBOL
                    && (string)currentToken.Value == "-"
                    && isExpressionStart
                    && tokens.Peek().Type == TokenTypeEnum.NUMBER)
                {
                    double val = -(double)tokens.Dequeue().Value;
                    newToken = new(TokenTypeEnum.NUMBER, val);
                    isExpressionStart = false;
                }
                else
                {
                    newToken = currentToken;
                    isExpressionStart = currentToken.Type == TokenTypeEnum.LEFT_PARENTHESIS;
                }

                resultTokens.Add(newToken);

            }


            return resultTokens;
        }
    }
}
