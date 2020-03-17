using System.Collections.Generic;

namespace woof.CodeAnalysis
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind {get; }

        // for parse tree
        public abstract IEnumerable<SyntaxNode>  GetChildren();
    }
}