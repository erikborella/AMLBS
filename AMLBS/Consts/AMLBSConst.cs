using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMLBS.Consts
{
    internal static class AMLBSConst
    {
        internal static List<Token> Define(Dictionary<string, double> consts,
                                           Queue<Token> rtokens,
                                           AMLBS amlbs)
        {
            if (rtokens.Peek().Type != TokenTypeEnum.WORD)
                throw new Exception("Expected a WORD in const definition");

            string constName = (string)rtokens.Dequeue().Value;

            if (rtokens.Dequeue().Type != TokenTypeEnum.ATTRIBUTION)
                throw new Exception("Expected a ATTRIBUTION in function definition");

            List<Token> expression = new();

            while (rtokens.Peek().Type != TokenTypeEnum.END_EXPRESSION)
            {
                expression.Add(rtokens.Dequeue());
            }

            rtokens.Dequeue();

            double val = (double) amlbs.Eval(expression);

            consts.Add(constName, val);

            return rtokens.ToList();
        }

        internal static List<Token> Eval(Dictionary<string, double> consts,
                                         Queue<Token> rtokens)
        {
            List<Token> resultExpression = new();

            while (rtokens.Count != 0)
            {
                var currentToken = rtokens.Dequeue();
                Token newToken;

                if (currentToken.Type == TokenTypeEnum.WORD
                    && consts.ContainsKey((string)currentToken.Value))
                    newToken = new(TokenTypeEnum.NUMBER, consts[(string)currentToken.Value]);

                else
                    newToken = currentToken;

                resultExpression.Add(newToken);
            }

            return resultExpression;
        }
    }
}
