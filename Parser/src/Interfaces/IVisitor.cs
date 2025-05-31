using PixelWallE.Lexer.src;
using PixelWallE.Parser.src.Enums;
namespace PixelWallE.Parser.src.Interfaces;

public interface IVisitor
{
    void Visit(IStatement statement, Coord coord);
    void AssignVisit(string identifier, Result value, Coord coord);
    void LabelVisit(string identifier, Coord coord);
    void GotoVisit(string targetLabel, Result? condition, Coord coord);
    void ActionVisit(string identifier, Result[] arguments, Coord coord);
    void CodeBlockVisit(IStatement[] lines);
    Result FunctionVisit(string identifier, Result[] arguments, Coord coord);
    Result VariableVisit(string identifier, Coord coord);
    Result BinaryVisit(Result left, BinaryOperationType op, Result right, Coord coord);
    Result UnaryVisit(Result argument, UnaryOperationType op, Coord coord);
    Result LiteralVisit(Result value, Coord coord);
    Result[] ParametersVisit(IExpression[] expressions);
}

