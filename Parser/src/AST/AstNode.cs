namespace PixelWallE.Parser.src.AST;

public interface ISearch
{
    void SearchLabel(Context context);
}

public abstract class AstNode : ISearch
{
    public virtual void Accept() { }
    public virtual void SearchLabel(Context context) { }
}
