namespace ProceduralDungeon.Pooling
{
    public interface IPoolable
    {
        void OnPoolCreated();
        void OnPoolGet();
        void OnPoolReturn();
    }
}