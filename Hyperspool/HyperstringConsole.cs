using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hyperspool
{
    public static class HyperstringConsole
    {
        private static void Main()
        {
            ConsoleWriteLine("Type '#help' for all pseudo-commands", ConsoleColor.Magenta);

            bool showtree = false;
            var variables = new Dictionary<VariableSymbol, object>();
            var textBuilder = new StringBuilder();
            Compilation previous = null;

            while (true)
            {
                if (textBuilder.Length == 0)
                    ConsoleWrite("» ", ConsoleColor.Green);
                else
                    ConsoleWrite("· ", ConsoleColor.Green);

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
                            case "#help":
                                Console.WriteLine();
                                ConsoleWrite("#tree: ", ConsoleColor.Yellow);
                                ConsoleWrite("Toggles showing of Parse Trees", ConsoleColor.DarkGreen);
                                Console.WriteLine();
                                ConsoleWrite("#cls: ", ConsoleColor.Yellow);
                                ConsoleWrite("Clears screen", ConsoleColor.DarkGreen);
                                Console.WriteLine();
                                ConsoleWrite("#mem: ", ConsoleColor.Yellow);
                                ConsoleWrite("Shows variables stored in memory and their values", ConsoleColor.DarkGreen);
                                Console.WriteLine();
                                ConsoleWrite("#clrmem: ", ConsoleColor.Yellow);
                                ConsoleWrite("Clears current scope", ConsoleColor.DarkGreen);
                                Console.WriteLine();

                                Console.WriteLine();
                                continue;

                            case "#tree":
                                Console.WriteLine();
                                showtree = !showtree;
                                ConsoleWriteLine(showtree ? "Showing Parse trees" : "Stopped showing Parse trees", ConsoleColor.Magenta);
                                Console.WriteLine();
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
                                previous = null;
                                ConsoleWriteLine("Cleared Scope", ConsoleColor.Magenta);
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

                Compilation compilation = previous == null ? new Compilation(syntaxTree) : previous.ContinueWith(syntaxTree);
                var result = compilation.Evaluate(variables);

                var diagnostics = result.Diagnostics;

                if (showtree)
                {
                    WriteSyntaxTreeTo(syntaxTree, Console.Out, ConsoleColor.DarkGray);
                }

                if (!diagnostics.Any())
                {
                    ConsoleWriteLine(result.Value, ConsoleColor.Green);
                    previous = compilation;
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

        static void WriteSyntaxTreeTo(SyntaxTree _tree, TextWriter _writer, ConsoleColor _color)
        {
            Console.ForegroundColor = _color;
            _tree.Root.WriteTo(Console.Out);
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
