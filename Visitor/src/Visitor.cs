using PixelWallE.Parser.src;
using PixelWallE.Parser.src.AST;
namespace PixelWallE.Visitor.src;

//TODO Hacer un IVisitor y Visitor para cada node del AST.

public interface IVisitor
{
    void Visit(StatementNode node);
    void Visit(ExpressionNode node);
    void Visit(AstNode node);
    void Visit(Block node);
    void Visit(AssignExpreNode node);
    void Visit(BinaryExpreNode node);
    void Visit(UnaryExpreNode node);
    void Visit(LiteralExpreNode node);
    void Visit(IdentifierExpreNode node);
    void Visit(FunctionCallExpreNode node);
}

public class ConcreteVisitor : IVisitor
{

}