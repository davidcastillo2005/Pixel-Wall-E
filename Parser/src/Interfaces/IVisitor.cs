using PixelWallE.Parser.src.Enums;
namespace PixelWallE.Parser.src.Interfaces;

public interface IVisitor
{
    void Visit(IStatement statement);
    void AssignVisit(string identifier, Result value);
    void LabelVisit(string identifier, int line);
    void GotoVisit(string targetLabel, Result? condition);
    void ActionVisit(string identifier, Result[] arguments);
    void CodeBlockVisit(IStatement[] lines);
    Result FuncVisit(string identifier, Result[] arguments);
    Result VariableVisit(string identifier);
    Result BinaryVisit(Result left, BinaryOperation op, Result right);
    Result UnaryVisit(Result argument, UnaryOperation op);
    Result LiteralVisit(Result value);
    Result[] ParamsVisit(IExpression[] expressions);
}

