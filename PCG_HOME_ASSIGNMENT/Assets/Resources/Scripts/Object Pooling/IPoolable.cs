namespace ProceduralDungeon.Pooling
{
    public interface IPoolable
    {
        void OnPoolCreate();
        void OnPoolGet();
        void OnPoolReturn();
    }
}