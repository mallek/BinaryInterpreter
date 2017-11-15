﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace interpreter
{
    class Program
    {
        static List<Token> Lex(string input)
        {
            var result = new List<Token>();
            for (int i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case '+':
                        result.Add(new Token(Token.Type.Plus, "+"));
                        break;
                    case '-':
                        result.Add(new Token(Token.Type.Minus, "-"));
                        break;
                    case '(':
                        result.Add(new Token(Token.Type.Lparen, "("));
                        break;
                    case ')':
                        result.Add(new Token(Token.Type.Rparen, ")"));
                        break;
                    default:
                        var sb = new StringBuilder(input[i].ToString());
                        for (int j = i + 1; j < input.Length; ++j)
                        {
                            if (char.IsDigit(input[j]))
                            {
                                sb.Append(input[j]);
                                ++i;
                            }
                            else
                            {
                                result.Add(new Token(Token.Type.Integer, sb.ToString()));
                                break;
                            }
                        }
                        break;
                }
            }
            return result;
        }

        static IElement Parse(IReadOnlyList<Token> tokens)
        {
            var result = new BinaryOperation();
            bool hasLeftHandSide = false;
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                switch (token.MyType)
                {
                    case Token.Type.Integer:
                        var integer = new Integer(int.Parse(token.Text));
                        if (!hasLeftHandSide)
                        {
                            result.Left = integer;
                            hasLeftHandSide = true;
                        }
                        else
                        {
                            result.Right = integer;
                        }
                        break;
                    case Token.Type.Plus:
                        result.MyType = BinaryOperation.Type.Addition;
                        break;
                    case Token.Type.Minus:
                        result.MyType = BinaryOperation.Type.Subtraction;
                        break;
                    case Token.Type.Lparen:
                        int j = i;
                        for (; j < tokens.Count; ++j)
                        {
                            if (tokens[j].MyType == Token.Type.Rparen)
                                break;
                        }

                        var subexpression = tokens.Skip(i + 1).Take(j - i - 1).ToList();
                            var element = Parse(subexpression);
                            if (!hasLeftHandSide)
                            {
                                result.Left = element;
                                hasLeftHandSide = true;
                            }
                            else
                            {
                                result.Right = element;
                            }
                        
                        i = j;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return result;
        }

        static void Main(string[] args)
        {
            string input = "(131+42+980)-(112+1500)";
            var tokens = Lex(input);
            Console.WriteLine(string.Join("\t", tokens));

            var parsed = Parse(tokens);
            Console.WriteLine($"{input} = {parsed.Value}");
        }
    }
}
