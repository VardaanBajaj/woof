using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using woof.CodeAnalysis;
using woof.CodeAnalysis.Binding;
using woof.CodeAnalysis.Syntax;
using woof.CodeAnalysis.Text;

namespace woof
{
    internal static class Program
    {
        private static void Main()
        {
            var showTree = false;
            var variables = new Dictionary<VariableSymbol, object>();
            var textBuilder = new StringBuilder();
            while(true)
            {
                if(textBuilder.Length == 0)
                    Console.Write("> ");
                else
                    Console.Write("| ");

                var input = Console.ReadLine();
                var isBlank = string.IsNullOrWhiteSpace(input);

                if(textBuilder.Length == 0)
                {
                    if(isBlank)
                        break;
                    else if(input == "#cls")
                    {
                        Console.Clear();
                        continue;
                    }
                    else if(input == "#showTree")
                    {
                        showTree = !showTree;
                        Console.WriteLine(showTree ? "Showing Parse Tree" : "Not showing parse tree");
                        continue;
                    }
                }

                textBuilder.AppendLine(input);
                var text = textBuilder.ToString();

                var syntaxTree = SyntaxTree.Parse(text);

                if(!isBlank && syntaxTree.Diagnostics.Any())
                    continue;

                var compilation = new Compilation(syntaxTree);
                var result = compilation.Evaluate(variables);
                var diagnostics = result.Diagnostics;

                if(showTree)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    syntaxTree.Root.WriteTo(Console.Out);
                    // PrettyPrint(syntaxTree.Root);
                    Console.ResetColor();
                }
                if (!diagnostics.Any())
                {
                    Console.WriteLine(result.Value);
                }
                else
                {
                    foreach (var diagnostic in diagnostics)
                    {

                        var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
                        var line = syntaxTree.Text.Lines[lineIndex];
                        var lineNumber = lineIndex + 1;
                        var character = diagnostic.Span.Start - line.Start + 1;
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write($"({lineNumber}, {character}): ");
                        Console.WriteLine(diagnostic);
                        Console.ResetColor();

                        var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
                        var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

                        var prefix = syntaxTree.Text.ToString(prefixSpan);
                        var error = syntaxTree.Text.ToString(diagnostic.Span);
                        var suffix = syntaxTree.Text.ToString(suffixSpan);

                        Console.Write("    ");
                        Console.Write(prefix);

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(error);
                        Console.ResetColor();

                        Console.Write(suffix);
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
                textBuilder.Clear();
            }
        }

        // static void PrettyPrint(SyntaxNode node, string indent="", bool isLast=true)
        // {
        //     // ├──

        //     // │

        //     // └──

        //     var marker = isLast ? "└──": "├──" ;

        //     Console.Write(indent);
        //     Console.Write(marker);
        //     Console.Write(node.Kind);
        //     if(node is SyntaxToken t && t.Value != null)
        //     {
        //         Console.Write(" ");
        //         Console.Write(t.Value);
        //     }
        //     Console.WriteLine();


        //     indent += isLast ? "   " : "│  " ;

        //     var lastChild=node.GetChildren().LastOrDefault();


        //     foreach(var child in node.GetChildren())
        //     {
        //         PrettyPrint(child, indent, child==lastChild);
        //     }
        // }
    }

}
