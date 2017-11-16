using System;
using System.Collections.Generic;
using System.Text;


namespace TripleZero.Helper.Caching
{
    public interface ICaching<T> : ICaching
    {
        T Init();
    }

    public interface ICaching
    {
        string Name();
        int Minutes();
    }
}
