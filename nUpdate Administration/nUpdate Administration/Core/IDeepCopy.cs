namespace nUpdate.Administration.Core
{
    public interface IDeepCopy<out T> where T : class
    {
        T DeepCopy();
    }
}