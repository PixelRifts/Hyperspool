using System;
using System.Collections.Generic;
using System.Linq;

namespace Hyperspool
{
    public static class Program
    {
        private static void Main()
        {
            bool showtree = false;
            var variables = new Dictionary<VariableSymbol, object>();

            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) return;

                switch (line)
                {
                    case "#tree":
                        showtree = !showtree;
                        ConsoleWriteLine(showtree ? "Showing Parse trees" : "Stopped showing Parse trees", ConsoleColor.Magenta);
                        continue;
                    case "#cls":
                        Console.Clear();
                        continue;
                    case "#mem":
                        Console.WriteLine();
                        if (!variables.Any()) ConsoleWriteLine("No Variable Stored in Memory", ConsoleColor.Magenta);

                        foreach (var variable in variables.Keys)
                        {
                            ConsoleWrite(variable.Name, ConsoleColor.Yellow);
                            ConsoleWrite(" = ", ConsoleColor.Gray);
                            ConsoleWrite(variables.GetValueOrDefault(variable), ConsoleColor.DarkGreen);
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                        continue;
                }

                var syntaxTree = SyntaxTree.Parse(line);
                Compilation compilation = new Compilation(syntaxTree);
                var result = compilation.Evaluate(variables);

                var diagnostics = result.Diagnostics;

                if (showtree)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    PrettyPrint(syntaxTree.Root);
                    Console.ResetColor();
                }

                if (!diagnostics.Any())
                {
                    ConsoleWriteLine(result.Value, ConsoleColor.Green);
                }
                else
                {
                    foreach (var diag in diagnostics)
                    {
                        Console.WriteLine();
                        ConsoleWriteLine(diag, ConsoleColor.DarkRed);

                        var prefix = line.Substring(0, diag.Span.Start);
                        var error = line.Substring(diag.Span.Start, diag.Span.Length);
                        var suffix = line.Substring(diag.Span.End);

                        Console.Write("    ");
                        Console.Write(prefix);
                        ConsoleWrite(error, ConsoleColor.Red);
                        Console.Write(suffix);

                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
            }
        }



        static void ConsoleWrite(object _text, ConsoleColor _color)
        {
            Console.ForegroundColor = _color;
            Console.Write(_text);
            Console.ResetColor();
        }

        static void ConsoleWriteLine(object _text, ConsoleColor _color)
        {
            Console.ForegroundColor = _color;
            Console.WriteLine(_text);
            Console.ResetColor();
        }                                                                                            


        static void PrettyPrint(SyntaxNode _node, string _indent = "", bool isLast = true)
        {
            var _marker = isLast ? "└──" : "├──";

            Console.Write(_indent);
            Console.Write(_marker);
            Console.Write(_node.Kind);

            if (_node is SyntaxToken _t && _t.Value != null)
            {
                Console.Write(" ");
                Console.Write(_t.Value);
            }
            Console.WriteLine();

            _indent += isLast ? "    " : "│   ";
            var _lastChild = _node.GetChildren().LastOrDefault();

            foreach (var _child in _node.GetChildren())
            {
                PrettyPrint(_child, _indent, _child == _lastChild);
            }
        }
    }
}
