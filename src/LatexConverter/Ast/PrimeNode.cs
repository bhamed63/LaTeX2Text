using System.Collections.Generic;

namespace LatexConverter
{
    /// <summary>
    /// Represents a prime symbol applied to a base node.
    /// </summary>
    public record PrimeNode(AstNode Base, int Count) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitPrime(this);

        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitPrime(this);

        public override List<AstNode> GetAllSubNodes()
        {
            return new List<AstNode> { Base };
        }

        public override string CreateCommandName()
        {
            return ToString();
        }

        public override string ToString()
        {
            var primeString = new System.Text.StringBuilder();
            for (int i = 0; i < Count; i++)
            {
                primeString.Append("\\prime");
            }
            return $"{Base}{primeString}";
        }
    }
}
