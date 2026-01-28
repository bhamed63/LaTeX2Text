using LatexConverter.Ast;

namespace LatexConverter
{
    /// <summary>
    /// Defines the interface for a visitor that traverses the AST.
    /// </summary>
    public interface IVisitor<T>
    {
        T VisitText(TextNode node);
        T ExceptionalVisitText(TextNode node);
        T VisitCommand(CommandNode node);
        T ExceptionalVisitCommand(CommandNode node);
        T VisitGroup(GroupNode node);
        T ExceptionalVisitGroup(GroupNode node);
        T VisitScript(ScriptNode node);
        T ExceptionalVisitScript(ScriptNode node);
        T VisitMatrix(MatrixNode node);
        T ExceptionalVisitMatrix(MatrixNode node);
        T VisitRoot(RootNode node);
        T ExceptionalVisitRoot(RootNode node);
        T VisitFrac(FracNode node);
        T ExceptionalVisitFrac(FracNode node);
        T VisitBinom(BinomNode node);
        T ExceptionalVisitBinom(BinomNode node);
        T VisitMath(MathNode node);
        T ExceptionalVisitMath(MathNode node);
        T VisitLim(LimNode node);
        T ExceptionalVisitLim(LimNode node);
        T VisitRelationalOperator(RelationalOperatorNode node);
        T ExceptionalVisitRelationalOperator(RelationalOperatorNode node);
        T VisitAbsoluteValue(AbsoluteValueNode node);
        T ExceptionalVisitAbsoluteValue(AbsoluteValueNode node);
        T VisitPrescript(PrescriptNode node);
        T ExceptionalVisitPrescript(PrescriptNode node);
        T VisitPrime(PrimeNode node);
        T ExceptionalVisitPrime(PrimeNode node);
    }
}
