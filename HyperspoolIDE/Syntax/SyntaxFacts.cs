﻿using System;
using System.Collections.Generic;

namespace Hyperspool
{
    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.BangToken:
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken: return 6;


                default: return 0;
            }
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PipePipeToken: return 1;

                case SyntaxKind.AmpersandAmpersandToken: return 2;

                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.BangEqualsToken: return 3;

                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken: return 4;

                case SyntaxKind.StarToken:
                case SyntaxKind.SlashToken: return 5;

                default: return 0;
            }
        }

        public static IEnumerable<SyntaxKind> GetUnaryOperatorKinds()
        {
            var _kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach (var _kind in _kinds)
            {
                if (GetUnaryOperatorPrecedence(_kind) > 0)
                    yield return _kind;
            }
        }

        public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds()
        {
            var _kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach (var _kind in _kinds)
            {
                if (GetBinaryOperatorPrecedence(_kind) > 0)
                    yield return _kind;
            }
        }

        public static SyntaxKind GetKeywordKind(string text)
        {
            switch (text)
            {
                case "true": return SyntaxKind.TrueKeyword;
                case "false": return SyntaxKind.FalseKeyword;
                default: return SyntaxKind.IdentifierToken;
            }
        }

        public static string GetText(SyntaxKind _kind)
        {
            switch (_kind)
            {
                case SyntaxKind.EqualsToken: return "=";
                case SyntaxKind.PlusToken: return "+";
                case SyntaxKind.MinusToken: return "-";
                case SyntaxKind.StarToken: return "*";
                case SyntaxKind.SlashToken: return "/";
                case SyntaxKind.OpenParenthesisToken: return "(";
                case SyntaxKind.CloseParenthesisToken: return ")";
                case SyntaxKind.BangToken: return "!";
                case SyntaxKind.AmpersandAmpersandToken: return "&&";
                case SyntaxKind.PipePipeToken: return "||";
                case SyntaxKind.EqualsEqualsToken: return "==";
                case SyntaxKind.BangEqualsToken: return "!=";
                case SyntaxKind.TrueKeyword: return "true";
                case SyntaxKind.FalseKeyword: return "false";
                default: return null;
            }
        }
    }
}
