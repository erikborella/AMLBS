using AMLBS.Expressions;
using AMLBS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMLBS.Functions
{
    internal static class AMLBSFunction
    { 

        #region define
        internal static List<Token> Define(Dictionary<string, FunctionDefinition> functions,
                                           Queue<Token> rtokens)
        {

            if (rtokens.Peek().Type != TokenTypeEnum.WORD)
                throw new Exception("Expected a WORD in function definition");

            string funName = (string)rtokens.Dequeue().Value;
            FunctionDefinition functionDefinition = GetFunctionDefinition(rtokens);

            functions.Add(funName, functionDefinition);

            return rtokens.ToList();
        }

        internal static FunctionDefinition GetFunctionDefinition(Queue<Token> rtokens)
        {
            var parameters = ExtractFunctionDefineParameters(rtokens);

            if (rtokens.Dequeue().Type != TokenTypeEnum.ATTRIBUTION)
                throw new Exception("Expected a ATTRIBUTION in function definition");

            List<Token> expression = new();
            ReplaceDefineParameters(rtokens, parameters, expression);

            rtokens.Dequeue();

            FunctionDefinition functionDefinition = new(parameters.Count, expression);
            return functionDefinition;
        }

        private static void ReplaceDefineParameters(Queue<Token> rtokens,
                                                    Dictionary<string, string> parameters,
                                                    List<Token> expression)
        {
            while (rtokens.Peek().Type != TokenTypeEnum.END_EXPRESSION)
            {
                Token currentToken = rtokens.Dequeue();

                if (currentToken.Type == TokenTypeEnum.WORD &&
                    parameters.ContainsKey((string)currentToken.Value))
                {
                    var paremeterName = parameters[(string)currentToken.Value];
                    expression.Add(new Token(TokenTypeEnum.NUMBER, paremeterName));
                }
                else
                    expression.Add(currentToken);
            }
        }

        private static Dictionary<string, string> ExtractFunctionDefineParameters(Queue<Token> rtokens)
        {
            if (rtokens.Dequeue().Type != TokenTypeEnum.LEFT_PARENTHESIS)
                throw new Exception("Expected a LEFT_PARENTHESIS after function name");

            int parametersCount = 0;
            Dictionary<string, string> parameters = new();

            while (rtokens.Peek().Type != TokenTypeEnum.RIGHT_PARENTHESIS)
            {
                if (rtokens.Peek().Type != TokenTypeEnum.WORD)
                    throw new Exception("Expected a WORD in parameter name");

                parametersCount++;

                string parameterName = (string)rtokens.Dequeue().Value;
                string parameterInternalName = $"${parametersCount}";

                parameters.Add(parameterName, parameterInternalName);

                if (rtokens.Peek().Type != TokenTypeEnum.SEPARATOR &&
                    rtokens.Peek().Type != TokenTypeEnum.RIGHT_PARENTHESIS)
                    throw new Exception("Expected a SEPARATOR afeter parameter name");

                if (rtokens.Peek().Type == TokenTypeEnum.SEPARATOR)
                    rtokens.Dequeue();
            }

            rtokens.Dequeue();

            return parameters;
        }

        #endregion

        #region eval

        internal static double EvalFunction(string funName,
                                            Queue<Token> rtokens,
                                            AMLBS amlbs)
        {

            if (funName == "if")
                return EvalIf(rtokens, amlbs);
            else if (amlbs.functions.ContainsKey(funName))
                return EvalAMLBSFunction(funName, rtokens, amlbs);
            else if (amlbs.nativeFunctions.ContainsKey(funName))
                return EvalNativeFunction(funName, rtokens, amlbs);
            else
                throw new Exception($"Function name {funName} not defined");
            
        }

        private static double EvalAMLBSFunction(string funName,
                                                Queue<Token> rtokens,
                                                AMLBS amlbs)
        {
            FunctionDefinition functionDefinition = amlbs.functions[funName];

            List<double> parameters = ExtractFunctionEvalParameters(rtokens, amlbs);

            if (parameters.Count != functionDefinition.ParametersCount)
                throw new Exception($"function {funName} " +
                    $"expected {functionDefinition.ParametersCount} parameter " +
                    $"but found {parameters.Count}");

            List<Token> functionExpression = new(functionDefinition.tokens);
            ExpressionParametersReplacer.ReplaceEvalParameters(parameters, functionExpression);

            return (double)amlbs.Eval(functionExpression);
        }

        private static double EvalNativeFunction(string funName,
                                                 Queue<Token> rtokens,
                                                 AMLBS amlbs)
        {
            NativeFunctionDefinition nativeDefinition = amlbs.nativeFunctions[funName];

            List<double> parameters = ExtractFunctionEvalParameters(rtokens, amlbs);

            if (parameters.Count != nativeDefinition.ParametersCount)
                throw new Exception($"native function {funName} " +
                    $"expected {nativeDefinition.ParametersCount} parameter " +
                    $"but found {parameters.Count}");

            return nativeDefinition.NativeCall(parameters.ToArray());
        }

        private static double EvalIf(Queue<Token> rtokens,
                                     AMLBS amlbs)
        {
            var parameters = ExtractExpressionParameters(rtokens);

            double conditionResult = (double) amlbs.Eval(parameters[0]);

            if (conditionResult != 1)
                return (double)amlbs.Eval(parameters[2]);
            else
                return (double)amlbs.Eval(parameters[1]);

        }

        private static List<double> ExtractFunctionEvalParameters(Queue<Token> rtokens,
                                                                  AMLBS amlbs)
        {
            if (rtokens.Dequeue().Type != TokenTypeEnum.LEFT_PARENTHESIS)
                throw new Exception("Expected a LEFT_PARENTHESIS after function name");

            List<double> parameters = new();
            int insideExpressionCount = 0;

            while (rtokens.Peek().Type != TokenTypeEnum.RIGHT_PARENTHESIS
                || insideExpressionCount != 0)
            {
                List<Token> insideExpression = new();

                while (rtokens.Peek().Type != TokenTypeEnum.SEPARATOR
                    && rtokens.Peek().Type != TokenTypeEnum.RIGHT_PARENTHESIS
                    || insideExpressionCount != 0)
                {
                    var token = rtokens.Dequeue();

                    if (token.Type == TokenTypeEnum.LEFT_PARENTHESIS)
                        insideExpressionCount++;

                    else if (token.Type == TokenTypeEnum.RIGHT_PARENTHESIS)
                        insideExpressionCount--;

                    insideExpression.Add(token);
                }

                if (rtokens.Peek().Type == TokenTypeEnum.SEPARATOR)
                    rtokens.Dequeue();

                double result = (double) amlbs.Eval(insideExpression);

                parameters.Add(result);
            }

            rtokens.Dequeue();

            return parameters;
        }

        private static List<List<Token>> ExtractExpressionParameters(Queue<Token> rtokens)
        {
            if (rtokens.Dequeue().Type != TokenTypeEnum.LEFT_PARENTHESIS)
                throw new Exception("Expected a LEFT_PARENTHESIS after function name");

            List<List<Token>> parameters = new();
            int insideExpressionCount = 0;

            while (rtokens.Peek().Type != TokenTypeEnum.RIGHT_PARENTHESIS
                || insideExpressionCount != 0)
            {
                List<Token> insideExpression = new();

                while (rtokens.Peek().Type != TokenTypeEnum.SEPARATOR
                    && rtokens.Peek().Type != TokenTypeEnum.RIGHT_PARENTHESIS
                    || insideExpressionCount != 0)
                {
                    var token = rtokens.Dequeue();

                    if (token.Type == TokenTypeEnum.LEFT_PARENTHESIS)
                        insideExpressionCount++;

                    else if (token.Type == TokenTypeEnum.RIGHT_PARENTHESIS)
                        insideExpressionCount--;

                    insideExpression.Add(token);
                }

                if (rtokens.Peek().Type == TokenTypeEnum.SEPARATOR)
                    rtokens.Dequeue();

                parameters.Add(insideExpression);
            }

            rtokens.Dequeue();

            return parameters;
        }

        #endregion
    }
}
