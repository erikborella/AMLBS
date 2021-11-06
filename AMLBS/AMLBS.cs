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
