using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaBook.Classes
{
    public class Equation
    {
        public static string MoveToLeft(string equation)
        {
            string leftSide = equation.Split("=")[0];
            string rightSide = equation.Split("=")[1];
            int openParentheses = 0;
            for (int i = 0; i < rightSide.Length; i++)
            {
                if (rightSide[i] == '(')
                {
                    openParentheses++;
                }

                if (rightSide[i] == ')')
                {
                    openParentheses--;
                }

                if (i == 0 && rightSide[i] != '-')
                {
                    leftSide += "-";
                }

                if (rightSide[i] == '+' && openParentheses == 0)
                {
                    leftSide += "-";
                }
                else if (rightSide[i] == '-' && openParentheses == 0)
                {
                    leftSide += "+";
                }
                else
                {
                    leftSide += rightSide[i];
                }
            }

            return leftSide + "=0";
        }

        public static double? Evaluate(string expression)
        {
            expression = expression.Replace("--", "+");
            expression = RemoveExp(expression);
            //Console.WriteLine("CheckPoint 0, " + expression);
            while (expression.Contains('('))
            {
                int st = expression.IndexOf('(');
                int en = IndexOfBalancedClosingParentheses(expression);
                string block = expression.Substring(st, en - st + 1);
                string center = expression.Substring(st + 1, en - st - 1);
                expression = expression.Replace(block, Evaluate(center).ToString());
            }

            expression = RemoveExp(expression);
            //Console.WriteLine("CheckPoint 1, " + expression);
            expression = RemoveExp(expression);
            while (expression.Contains("^"))
            {
                int indexOfConcern = expression.IndexOf("^");
                string chunk = GetFirstChunkOfOperator(expression, '^');
                if (chunk.Length > 0)
                {
                    int center = chunk.IndexOf('^');
                    double first = SpecialParse(chunk.Substring(0, center));
                    double second = SpecialParse(chunk.Substring(center + 1));
                    expression = expression.Replace(chunk, Math.Pow(first, second).ToString());
                }

                expression = RemoveExp(expression);
            }

            expression = RemoveExp(expression);
            while (expression.Contains("*") || expression.Contains("/"))
            {
                int multiplyIndex = expression.IndexOf("*");
                if (multiplyIndex < 0)
                    multiplyIndex = expression.Length;
                int divIndex = expression.IndexOf("/");
                if (divIndex < 0)
                    divIndex = expression.Length;
                int indexOfConcern = Math.Min(multiplyIndex, divIndex);
                if (expression[indexOfConcern] == '*')
                {
                    string chunk = GetFirstChunkOfOperator(expression, '*');
                    if (chunk.Length > 0)
                    {
                        int center = chunk.IndexOf('*');
                        double first = SpecialParse(chunk.Substring(0, center));
                        double second = SpecialParse(chunk.Substring(center + 1));
                        expression = expression.Replace(chunk, (first * second).ToString());
                    }
                }
                else if (expression[indexOfConcern] == '/')
                {
                    string chunk = GetFirstChunkOfOperator(expression, '/');
                    if (chunk.Length > 0)
                    {
                        int center = chunk.IndexOf('/');
                        double first = SpecialParse(chunk.Substring(0, center));
                        double second = SpecialParse(chunk.Substring(center + 1));
                        if (second == 0)
                        {
                            return null;
                        }

                        expression = expression.Replace(chunk, (first / second).ToString());
                    }
                }

                expression = RemoveExp(expression);
            }

            expression = expression.Replace("--", "+");
            expression = RemoveExp(expression);
            //Console.WriteLine("Checkpoint 2: " + expression);
            while (expression.Contains("+") || expression.Substring(1).Contains("-"))
            {
                int sumIndex = expression.IndexOf("+");
                if (sumIndex < 0)
                    sumIndex = expression.Length;
                int subIndex = expression.Substring(1).IndexOf("-") + 1;
                if (subIndex <= 0)
                    subIndex = expression.Length;
                int indexOfConcern = Math.Min(sumIndex, subIndex);
                if (expression[indexOfConcern] == '+')
                {
                    string chunk = GetFirstChunkOfOperator(expression, '+');
                    if (chunk.Length > 0)
                    {
                        int center = chunk.IndexOf('+');
                        double first = SpecialParse(chunk.Substring(0, center));
                        double second = SpecialParse(chunk.Substring(center + 1));
                        expression = expression.Replace(chunk, (first + second).ToString());
                    }
                }
                else if (expression[indexOfConcern] == '-')
                {
                    string chunk = GetFirstChunkOfOperator(expression, '-');
                    if (chunk.Length > 0)
                    {
                        int center = chunk.Substring(1).IndexOf('-') + 1;
                        double first = SpecialParse(chunk.Substring(0, center));
                        double second = SpecialParse(chunk.Substring(center + 1));
                        expression = expression.Replace(chunk, (first - second).ToString());
                    }
                }

                expression = RemoveExp(expression);
            }

            //Console.WriteLine("Finished");
            return SpecialParse(expression);
        }

        public static string GetFirstChunkOfOperator(string expression, char operatorTodo)
        {
            expression = RemoveExp(expression);
            //Console.WriteLine("CheckPoint 3: " + expression + " -> " + operatorTodo);
            List<char> operators = new List<char>()
        {
            '*',
            '/',
            '+',
            '-',
            '^'
        };
            List<char> nonNumerical = new List<char>()
        {
            '*',
            '/',
            '+',
            '-',
            '^',
            '(',
            ')'
        };
            int indexOfConcern = expression.Substring(1).IndexOf(operatorTodo) + 1;
            if (indexOfConcern < 0)
                return "";
            int PreviousOperatorIndex = indexOfConcern - 2;
            while (PreviousOperatorIndex >= 0 && !operators.Contains((expression[PreviousOperatorIndex])))
            {
                PreviousOperatorIndex--;
            }

            if (PreviousOperatorIndex >= 0 && expression[PreviousOperatorIndex] == '-')
            {
                if (PreviousOperatorIndex == 0)
                {
                    //Console.WriteLine("Start Char Override");
                    PreviousOperatorIndex--;
                }
                else if (nonNumerical.Contains(expression[PreviousOperatorIndex - 1]))
                {
                    //Console.WriteLine("Negative Override");
                    PreviousOperatorIndex--;
                }
            }

            //Console.WriteLine("Checkpoint 3.4: POI: "+PreviousOperatorIndex+", IOC: "+indexOfConcern);
            double firstNum = SpecialParse(expression.Substring(PreviousOperatorIndex + 1, indexOfConcern - PreviousOperatorIndex - 1));
            //Console.WriteLine("Checkpoint 3.5: " + firstNum);
            int NextOperatorIndex = indexOfConcern + 2;
            while (NextOperatorIndex < expression.Length && !operators.Contains((expression[NextOperatorIndex])))
            {
                NextOperatorIndex++;
            }

            //Console.WriteLine("Checkpoint 3.8: " + indexOfConcern + "," + NextOperatorIndex);
            double secondNum = SpecialParse(expression.Substring(indexOfConcern + 1, NextOperatorIndex - indexOfConcern - 1));
            //Console.WriteLine("Checkpoint 3.9: " + secondNum);
            string chunk = firstNum.ToString() + operatorTodo + secondNum;
            //Console.WriteLine("CheckPoint 4: " + chunk);
            return RemoveExp(chunk);
        }

        public static int IndexOfBalancedClosingParentheses(string expression)
        {
            int opens = 0;
            int closes = 0;
            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == '(')
                    opens++;
                if (expression[i] == ')')
                {
                    closes++;
                    if (opens == closes)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public static string TryEquation(string equation, double val, string varName)
        {
            string expression = MoveToLeft(equation);
            expression = expression.Substring(0, expression.IndexOf("="));
            expression = expression.Replace(varName, val.ToString());
            double? result = Evaluate(expression);
            return result == 0 ? val + " works" : val + " fails";
        }

        public static double SolveEquation(string equation, string varName)
        {
            Debug.Print(equation);
            string expression = MoveToLeft(equation);
            expression = expression.Substring(0, expression.IndexOf("="));
            double? zeroVal = null;
            double guessPoint = 0;
            while (zeroVal == null)
            {
                guessPoint += 0.1f;
                zeroVal = Evaluate(expression.Replace(varName, guessPoint.ToString()));
                if (zeroVal == 0)
                    return guessPoint;
            }
            Console.WriteLine("Start Point: " + guessPoint);

            double leftEndpoint = guessPoint;
            double leftVal = (double)zeroVal;
            double rightEndpoint = guessPoint;
            double rightVal = (double)zeroVal;
            for (double delta = 0.001; delta <= 1000; delta *= 10)
            {
                for (int i = 10; i >= -10; i--)
                {
                    if (i == 0)
                        continue;
                    double test = guessPoint + i * delta;
                    //Console.WriteLine(test.ToString());
                    //Console.WriteLine(i+","+delta+"->"+expression.Replace(varName, test.ToString()));
                    double? guessVal = Evaluate(expression.Replace(varName, test.ToString()));
                    if (guessVal == null) continue;
                    if (guessVal == 0)
                        return test;
                    if (guessVal > 0 && zeroVal < 0)
                    {
                        rightEndpoint = test;
                        rightVal = (double)guessVal;
                        i = -11;
                        delta = 1001;
                    }

                    if (guessVal < 0 && zeroVal > 0)
                    {
                        leftEndpoint = test;
                        leftVal = (double)guessVal;
                        i = -11;
                        delta = 1001;
                    }
                }
            }

            Console.WriteLine("endpoints found: " + leftEndpoint + ", " + rightEndpoint);
            double res = leftEndpoint;
            for (int i = 0; i < 1000; i++)
            {
                double center = (leftEndpoint + rightEndpoint) / 2;
                res = center;
                //Console.WriteLine(expression.Replace(varName, center.ToString()));
                double? centerVal = Evaluate(expression.Replace(varName, center.ToString()));
                if (centerVal == null) continue;
                if (Math.Sign((double)centerVal) == Math.Sign(leftVal))
                {
                    leftVal = (double)centerVal;
                    leftEndpoint = center;
                }
                else
                {
                    rightVal = (double)centerVal;
                    rightEndpoint = center;
                }

                if (Math.Abs((double)centerVal) < 0.001)
                {
                    i = 1001;
                }
            }

            return 0.01 * Math.Round(100 * res);
        }

        private static double SpecialParse(string input)
        {
            //Console.WriteLine("Parsing: " + input);
            return double.Parse(input.Replace("<<", "E-").Replace(">>", "E"));
        }

        private static string RemoveExp(string str)
        {
            return str.Replace("E-", "<<").Replace("E", ">>").Replace("<<+", "<<").Replace(">>+", ">>");
        }
    }
}
