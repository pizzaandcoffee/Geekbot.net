namespace Geekbot.Core.Interactions
{
    public interface IInteractionBase
    {
        void BeforeExecute();
        void AfterExecute();
        void OnException();
    }
}