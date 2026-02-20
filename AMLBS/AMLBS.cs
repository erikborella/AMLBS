using AMLBS.Consts;
using AMLBS.Expressions;
using AMLBS.Functions;
using AMLBS.Operators;
using AMLBS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMLBS
{
    public class AMLBS
    {
        internal Dictionary<string, double> consts = new();

        internal Dictionary<string, FunctionDefinition> functions = new();
        internal Dictionary<string, NativeFunctionDefinition> nativeFunctions = new();
        internal Dictionary<int, HigherOrderFunctionDefinition> higherOrderFunctions = new();
        private int currentFunctionReferenceId = 1;

        internal Dictionary<string, OperatorDefinition> operators = new();
        internal Dictionary<string, NativeOperatorDefinition> nativeOperators = new();

        public AMLBS()
        {
            this.UseStd();
        }

        public double? Execute(string expression)
        {
            List<Token> tokens = Lexer.Parse(expression);

            return Eval(tokens);
        }

        public void DefineNativeFunction(string funName, int parametersCount, NativeCall nativeCall)
        {
            nativeFunctions.Add(funName, new(parametersCount, nativeCall));
        }

        internal double CreateFunctionReference(string functionName,
                                                List<double>? boundParameters = null)
        {
            if (!functions.ContainsKey(functionName) && !nativeFunctions.ContainsKey(functionName))
                throw new Exception($"Function name {functionName} not defined");

            int referenceId = currentFunctionReferenceId++;
            higherOrderFunctions[referenceId] =
                new(functionName, boundParameters ?? new List<double>());

            return referenceId;
        }

        internal double CreateClosure(double functionReference,
                                      List<double> boundParameters)
        {
            int baseReferenceId = Convert.ToInt32(functionReference);

            if (!higherOrderFunctions.ContainsKey(baseReferenceId))
                throw new Exception($"Function reference {functionReference} is not valid");

            HigherOrderFunctionDefinition functionDefinition = higherOrderFunctions[baseReferenceId];

            List<double> capturedParameters = new(functionDefinition.BoundParameters);
            capturedParameters.AddRange(boundParameters);

            int closureReferenceId = currentFunctionReferenceId++;
            higherOrderFunctions[closureReferenceId] =
                new(functionDefinition.FunctionName, capturedParameters);

            return closureReferenceId;
        }

        internal double InvokeFunctionReference(double functionReference,
                                                List<double> parameters)
        {
            int referenceId = Convert.ToInt32(functionReference);

            if (!higherOrderFunctions.ContainsKey(referenceId))
                throw new Exception($"Function reference {functionReference} is not valid");

            HigherOrderFunctionDefinition functionDefinition = higherOrderFunctions[referenceId];
            List<double> allParameters = new(functionDefinition.BoundParameters);
            allParameters.AddRange(parameters);

            return InvokeFunction(functionDefinition.FunctionName, allParameters);
        }

        internal double InvokeFunction(string funName,
                                       List<double> parameters)
        {
            if (functions.ContainsKey(funName))
            {
                FunctionDefinition functionDefinition = functions[funName];

                if (parameters.Count != functionDefinition.ParametersCount)
                    throw new Exception($"function {funName} " +
                        $"expected {functionDefinition.ParametersCount} parameter " +
                        $"but found {parameters.Count}");

                List<Token> functionExpression = new(functionDefinition.tokens);
                ExpressionParametersReplacer.ReplaceEvalParameters(parameters, functionExpression);

                return (double)Eval(functionExpression);
            }

            if (nativeFunctions.ContainsKey(funName))
            {
                NativeFunctionDefinition nativeDefinition = nativeFunctions[funName];

                if (nativeDefinition.ParametersCount >= 0
                    && parameters.Count != nativeDefinition.ParametersCount)
                    throw new Exception($"native function {funName} " +
                        $"expected {nativeDefinition.ParametersCount} parameter " +
                        $"but found {parameters.Count}");

                return nativeDefinition.NativeCall(parameters.ToArray());
            }

            throw new Exception($"Function name {funName} not defined");
        }

        public void DefineNativeOperator(string opName, int precedence, NativeOperatorCall nativeCall)
        {
            nativeOperators.Add(opName, new(precedence, nativeCall));
        }

        internal double? Eval(List<Token> tokens)
        {
            tokens = EvalDefines(new Queue<Token>(tokens));

            tokens = EvalConsts(new Queue<Token>(tokens));

            tokens = EvalFunctions(new Queue<Token>(tokens));

            return AMLBSExpression.EvalExpression(tokens, this);
        }

        private List<Token> EvalDefines(Queue<Token> tokens)
        {
            List<Token> ignoredTokens = new();

            while (tokens.Count != 0)
            {
                var token = tokens.Peek();

                if (IsDefine(token))
                    DoDefine(tokens);
                else
                    ignoredTokens.Add(tokens.Dequeue());
            }

            return ignoredTokens;
        }

        private List<Token> EvalConsts(Queue<Token> tokens)
        {
            return AMLBSConst.Eval(consts, tokens);
        }

        private List<Token> EvalFunctions(Queue<Token> tokens)
        {
            List<Token> ignoredTokens = new();

            while (tokens.Count != 0)
            {
                Token ignored;
                var token = tokens.Dequeue();

                if (token.Type == TokenTypeEnum.WORD
                    && tokens.Peek().Type == TokenTypeEnum.LEFT_PARENTHESIS)
                {
                    double functionResult =
                        AMLBSFunction.EvalFunction((string)token.Value, tokens, this);

                    ignored = new Token(TokenTypeEnum.NUMBER, functionResult);
                }
                else
                    ignored = token;

                ignoredTokens.Add(ignored);
            }

            return ignoredTokens;
        }

        private void DoDefine(Queue<Token> tokens)
        {
            tokens.Dequeue();
            switch ((string) tokens.Dequeue().Value)
            {
                case "function":
                    AMLBSFunction.Define(functions, tokens);
                    break;
                case "const":
                    AMLBSConst.Define(consts, tokens, this);
                    break;
                case "operator":
                    AMLBSOperator.Define(operators, tokens);
                    break;
                default:
                    throw new Exception("definition type invalid");
            }

        }

        private bool IsDefine(Token token)
        {
            return token.Type == TokenTypeEnum.WORD && 
                (string)token.Value == "define";
        }
    }
}
