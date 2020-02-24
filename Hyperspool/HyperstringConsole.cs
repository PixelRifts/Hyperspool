using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperspool
{
    public static class HyperstringConsole
    {
        private static void Main()
        {
            bool showtree = false;
            var variables = new Dictionary<VariableSymbol, object>();
            var textBuilder = new StringBuilder();

            while (true)
            {
                if (textBuilder.Length == 0)
                    Console.Write("> ");
                else
                    Console.Write("| ");

                var input = Console.ReadLine();

                var isBlank = string.IsNullOrWhiteSpace(input);

                if (textBuilder.Length == 0)
                {
                    if (isBlank)
                        break;
                    else
                    {
                        switch (input)
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
                            case "#clrmem":
                                Console.WriteLine();
                                variables.Clear();
                                ConsoleWriteLine("Cleared All Variables from memory", ConsoleColor.Magenta);
                                Console.WriteLine();
                                continue;
                        }
                    }
                }
                textBuilder.AppendLine(input);
                var text = textBuilder.ToString();

                var syntaxTree = SyntaxTree.Parse(text);
                if (!isBlank && syntaxTree.Diagnostics.Any())
                    continue;

                Compilation compilation = new Compilation(syntaxTree);
                var result = compilation.Evaluate(variables);

                var diagnostics = result.Diagnostics;

                if (showtree)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    syntaxTree.Root.WriteTo(Console.Out);
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
                        var lineindex = syntaxTree.Text.GetLineIndex(diag.Span.Start);
                        var line = syntaxTree.Text.Lines[lineindex];
                        var linenumber = lineindex + 1;
                        var character = diag.Span.Start - line.Start + 1;

                        Console.WriteLine();
                        ConsoleWrite($"({linenumber}, {character}): ", ConsoleColor.Red);
                        ConsoleWriteLine(diag, ConsoleColor.DarkRed);

                        var prefixSpan = TextSpan.FromBounds(line.Start, diag.Span.Start);
                        var suffixSpan = TextSpan.FromBounds(diag.Span.End, line.End);

                        var prefix = syntaxTree.Text.ToString(prefixSpan);
                        var error = syntaxTree.Text.ToString(diag.Span.Start, diag.Span.Length);
                        var suffix = syntaxTree.Text.ToString(suffixSpan);

                        Console.Write("    ");
                        Console.Write(prefix);
                        ConsoleWrite(error, ConsoleColor.Red);
                        Console.Write(suffix);

                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
                textBuilder.Clear();
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
    }
}
