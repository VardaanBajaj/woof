namespace woof.CodeAnalysis
{
    public enum SyntaxKind
    {
        //Tokens
        NumberToken,
        WhitespaceToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParanthesisToken,
        BadToken,
        EndOfFileToken,

        //Expressions
        LiteralExpression,
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression
    }
}