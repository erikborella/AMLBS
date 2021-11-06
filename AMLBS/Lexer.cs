using AMLBS.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMLBS
{
    internal static class Lexer
    {

        private static char[] symbols = { '+', '-', '*', '/', '^', '>', '<', '=', '!', '&', '|' };

        internal static List<Token> Parse(string expression)
        {
            Queue<char> expressionQueue = new(expression.ToCharArray());
            List<Token> tokens = new();

            while (expressionQueue.Count != 0)
            {
                char ch = expressionQueue.Peek();

                if (IsParenthesis(ch))
                {
                    TokenTypeEnum type = (ch == '(') ? 
                        TokenTypeEnum.LEFT_PARENTHESIS : 
                        TokenTypeEnum.RIGHT_PARENTHESIS;

                    Token t = new(type, expressionQueue.Dequeue());
                    tokens.Add(t);
                }
                else if (IsSymbol(ch))
                {
                    string symbol = ParseSymbol(expressionQueue);
                    TokenTypeEnum type = (symbol == "=") 
                        ? TokenTypeEnum.ATTRIBUTION 
                        : TokenTypeEnum.SYMBOL;

                    Token t = new(type, symbol);
                    tokens.Add(t);
                }
                else if (IsSeparator(ch))
                {
                    Token t = new(TokenTypeEnum.SEPARATOR, expressionQueue.Dequeue());
                    tokens.Add(t);
                }
                else if (IsAttribution(ch))
                {
                    Token t = new(TokenTypeEnum.ATTRIBUTION, expressionQueue.Dequeue());
                    tokens.Add(t);
                }
                else if (IsEndExpression(ch))
                {
                    Token t = new(TokenTypeEnum.END_EXPRESSION, expressionQueue.Dequeue());
                    tokens.Add(t);
                }
                else if (IsNumber(ch))
                {
                    double num = ParseNumber(expressionQueue);

                    Token t = new(TokenTypeEnum.NUMBER, num);
                    tokens.Add(t);
                }
                else if (IsWord(ch, true))
                {
                    string word = ParseWord(expressionQueue);

                    Token t = new(TokenTypeEnum.WORD, word);
                    tokens.Add(t);
                }
                else
                {
                    expressionQueue.Dequeue();
                }

            }

            return tokens.ToList();
        }

        private static double ParseNumber(Queue<char> expressionQueue)
        {
            string val = new(
                AutomatusAnalyser.Analyse(expressionQueue, (c) => IsNumber(c))
            );

            return Convert.ToDouble(val, CultureInfo.InvariantCulture);
        }

        private static string ParseWord(Queue<char> expressionQueue)
        {
            string val = new(
                AutomatusAnalyser.Analyse(expressionQueue, (c) => IsWord(c))
            );

            return val;
        }

        private static string ParseSymbol(Queue<char> expressionQueue)
        {
            string val = new(
                AutomatusAnalyser.Analyse(expressionQueue, (c) => IsSymbol(c))
            );

            return val;
        }

        private static bool IsNumber(char ch)
        {
            return char.IsDigit(ch) || ch == '.';
        }

        private static bool IsWord(char ch, bool isStart = false)
        {

            if (isStart)
                return char.IsLetter(ch);
            else
                return char.IsLetterOrDigit(ch);
        }

        private static bool IsSymbol(char ch)
        {
            return Array.IndexOf(symbols, ch) > -1;
        }

        private static bool IsEndExpression(char ch)
        {
            return ch == ';';
        }

        private static bool IsSeparator(char ch)
        {
            return ch == ',';
        }
        
        private static bool IsAttribution(char ch)
        {
            return ch == '=';
        }

        private static bool IsParenthesis(char ch)
        {
            return ch == '(' || ch == ')';
        }
    }
}
