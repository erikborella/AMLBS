using AMLBS.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMLBS.Expressions
{
    internal static class RPNConversor
    {
        private static Dictionary<char, int> precedences = new()
        {
            { '+', 1 },
            { '-', 1 },
            { '*', 2 },
            { '/', 2 },
        };

        internal static List<Token> Convert(List<Token> tokens, AMLBS amlbs)
        {
            Stack<Token> operatorStack = new();
            Queue<Token> outputQueue = new();

            foreach (Token token in tokens)
            {
                switch (token.Type)
                {
                    case TokenTypeEnum.NUMBER:
                        outputQueue.Enqueue(token);
                        break;

                    case TokenTypeEnum.SYMBOL:
                    case TokenTypeEnum.WORD:
                        while (operatorStack.Count != 0 && GetPrecedence(operatorStack.Peek(), amlbs) > GetPrecedence(token, amlbs))
                            outputQueue.Enqueue(operatorStack.Pop());

                        operatorStack.Push(token);
                        break;

                    case TokenTypeEnum.LEFT_PARENTHESIS:
                        operatorStack.Push(token);
                        break;

                    case TokenTypeEnum.RIGHT_PARENTHESIS:
                        while (operatorStack.Count != 0 && !(operatorStack.Peek().Type == TokenTypeEnum.LEFT_PARENTHESIS))
                            outputQueue.Enqueue(operatorStack.Pop());

                        operatorStack.Pop();
                        break;

                }
            }

            while (operatorStack.Count != 0)
                outputQueue.Enqueue(operatorStack.Pop());

            return outputQueue.ToList();
        }

        private static int GetPrecedence(Token token, AMLBS amlbs)
        {
            if (token.Type == TokenTypeEnum.LEFT_PARENTHESIS)
                return 0;

            string opName = System.Convert.ToString(token.Value);

            var operatorDefinition = 
                AMLBSOperator.FindOperatorDefinition(opName, amlbs);

            if (operatorDefinition == null)
                throw new Exception($"Operator {opName} not defined");

            return operatorDefinition.Precedence;
        }
    }
}
