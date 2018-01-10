public class Singleton<T> where T : new()
{
    protected static T _instance = default(T);
    public static T GetInstance()
    {
        if (_instance == null)
        {
            _instance = new T();
        }
        return _instance;
    }

    public virtual void Init()
    {
    }

    public virtual void Destroy()
    {
    }
}

