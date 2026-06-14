namespace WillowGen.py;

public interface ISymbol
{
    /**
     * the unrealscript path name for this symbol i.e., WillowGame.WillowPlayerController
     */
    public string ExportPathName();
    
    public bool CanBeReferenced();
}