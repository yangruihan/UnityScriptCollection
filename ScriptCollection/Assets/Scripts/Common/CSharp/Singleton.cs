namespace Common.CSharp
{
    /// <summary>
    /// Extends this class to become singleton class
    /// </summary>
    /// <typeparam name="T"> T is a Singleton class </typeparam>
    public class Singleton<T> where T : class, new()
    {
        private static readonly object LockObj = new object();
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObj)
                    {
                        if (_instance == null)
                            _instance = new T();
                    }
                }

                return _instance;
            }
        }

        public void Free()
        {
            this.FreeSingleton();
            _instance = default(T);
        }

        protected virtual void FreeSingleton()
        {
        }
    }
}
