using AMLBS.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMLBS
{
    internal static class StackMachine
    {
        internal static double Evaluate(List<Token> tokens, AMLBS amlbs)
        {
            Stack<double> memory = new();

            foreach (Token token in tokens)
            {
                switch (token.Type)
                {
                    case TokenTypeEnum.NUMBER:
                        memory.Push((double) token.Value);
                        break;

                    case TokenTypeEnum.SYMBOL:
                    case TokenTypeEnum.WORD:
                        EvaluateOperation(memory, token, amlbs);
                        break;
                }
            }

            if (memory.Count != 1)
                throw new Exception("Unhanded data");

            return (double)memory.Pop();
        }

        private static void EvaluateOperation(Stack<double> memory, Token token, AMLBS amlbs)
        {
            string opName = Convert.ToString(token.Value);

            double n2 = (double)memory.Pop();
            double n1 = (double)memory.Pop();

            //double result = EvaluateOperation(n1, n2, (char) token.Value);
            double result = AMLBSOperator.EvalOperator(opName, n1, n2, amlbs);

            memory.Push(result);
        }

        private static double EvaluateOperation(double n1, double n2, char op)
        {
            switch (op)
            {
                case '+':
                    return n1 + n2;
                case '-':
                    return n1 - n2;
                case '*':
                    return n1 * n2;
                case '/':
                    return n1 / n2;
            }

            throw new Exception("Operation invalid");
        }
    }
}
