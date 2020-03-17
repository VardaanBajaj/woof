using System.Collections.Generic;

namespace woof.CodeAnalysis
{
    public sealed class ParanthesizedExpressionSyntax : ExpressionSyntax
    {
        public ParanthesizedExpressionSyntax(SyntaxToken openParanthesisToken, ExpressionSyntax expression, SyntaxToken closeParanthesisToken)
        {
            OpenParanthesisToken = openParanthesisToken;
            Expression = expression;
            CloseParanthesisToken = closeParanthesisToken;
        }

        public SyntaxToken OpenParanthesisToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParanthesisToken { get; }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParanthesisToken;
            yield return Expression;
            yield return CloseParanthesisToken;
        }
    }
}