using AMLBS.Functions;
using AMLBS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMLBS.Operators
{
    internal static class AMLBSOperator
    {
        internal static List<Token> Define(Dictionary<string, OperatorDefinition> operators,
                                           Queue<Token> rtokens)
        {

            if (rtokens.Dequeue().Type != TokenTypeEnum.LEFT_PARENTHESIS)
                throw new Exception("Expected a LEFT_PARENTHESIS in precedence operator definition");

            if (rtokens.Peek().Type != TokenTypeEnum.NUMBER)
                throw new Exception("Expected a NUMBER in precedence operator definition");

            int predecende = Convert.ToInt32(rtokens.Dequeue().Value);

            if (rtokens.Dequeue().Type != TokenTypeEnum.RIGHT_PARENTHESIS)
                throw new Exception("Expected a RIGHT_PARENTHESIS in precedence operator definition");


            if (rtokens.Peek().Type != TokenTypeEnum.WORD)
                throw new Exception("Expected a WORD in operator definition");

            string opName = (string)rtokens.Dequeue().Value;

            FunctionDefinition tempFunction = AMLBSFunction.GetFunctionDefinition(rtokens);
            if (tempFunction.ParametersCount != 2)
                throw new Exception($"Operator only accept 2 parameters, " +
                    $"but found {tempFunction.ParametersCount}");

            OperatorDefinition operatorDefinition = new(tempFunction.tokens, predecende);

            operators.Add(opName, operatorDefinition);

            return rtokens.ToList();
        }

        internal static BaseOperatorDefinition FindOperatorDefinition(string opName, AMLBS amlbs)
        {
            if (amlbs.operators.ContainsKey(opName))
                return amlbs.operators[opName];
            else if (amlbs.nativeOperators.ContainsKey(opName))
                return amlbs.nativeOperators[opName];
            else
                return null;
        }

        internal static double EvalOperator(string opName, double l, double r, AMLBS amlbs)
        {
            if (amlbs.operators.ContainsKey(opName))
            {
                return EvalAMLBSOperator(opName, l, r, amlbs);
            }
            else if (amlbs.nativeOperators.ContainsKey(opName))
            {
                return EvalNativeOperator(opName, l, r, amlbs);
            }
            else
                throw new Exception($"Function name {opName} not defined");
        }

        private static double EvalAMLBSOperator(string opName, double l, double r, AMLBS amlbs)
        {
            OperatorDefinition operatorDefinition = amlbs.operators[opName];

            List<Token> operatorExpression = new(operatorDefinition.Tokens);

            ExpressionParametersReplacer.ReplaceEvalParameters(new List<double>() { l, r },
                                                               operatorExpression);

            return (double)amlbs.Eval(operatorExpression);
        }

        private static double EvalNativeOperator(string opName, double l, double r, AMLBS amlbs)
        {
            NativeOperatorDefinition nativeDefinition = amlbs.nativeOperators[opName];
            return nativeDefinition.NativeCall(l, r);
        }
    }
}
