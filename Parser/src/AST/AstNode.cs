namespace PixelWallE.Parser.src.AST;

//TODO Buscar manera de agrupar las clases Expression y Statement dentro AstNode aún teniendo distintos Accept().
public interface ISearch
{
    void SearchLabel(Context context);
}

public abstract class AstNode : ISearch
{
    public virtual void Accept() { }
    public virtual void SearchLabel(Context context) { }
}
