namespace WillowGen.py;

public interface ISymbol
{
    /**
     * the unique name for this symbol, currently this is just the unreal path i.e.,
     * WillowGame.WillowPlayerController
     */
    public string ExportPathName();
    
    /**
     * can this symbol be referenced by a PyRef?
     */
    public bool CanBeReferenced();
}