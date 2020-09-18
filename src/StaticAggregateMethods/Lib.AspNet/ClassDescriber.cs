using System;
using System.Collections.Generic;

namespace Lib.AspNet
{
    public static class Describe
    {
        private static readonly Dictionary<Type, object> s_types = new Dictionary<Type, object>();

        public static IClassDescriber<T> Class<T>() where T : class
        {
            if (s_types.TryGetValue(typeof(T), out var obj) && obj is IClassDescriber<T> describer)
            {
                return describer;
            }

            describer = new ClassDescriber<T>();
            s_types[typeof(T)] = describer;

            return describer;
        }



        private class ClassDescriber<T> : IClassDescriber<T> where T : class
        {
            private readonly Dictionary<Func<T, object>, string> _urls = new Dictionary<Func<T, object>, string>();

            public IPropertyDescriber<T> Property(Func<T, object> func) => new PropertyDescriber(this, func);
            public IPropertyDescriber<T> Property(Func<T, string> func) => new PropertyDescriber(this, func);
            public ICollectionDescriber<T> Property<P>(Func<T, IEnumerable<P>> func) => new CollectionDescriber(this, func);

            private class PropertyDescriber : IPropertyDescriber<T>
            {
                private readonly ClassDescriber<T> _parent;
                private readonly Func<T, object> _func;

                public PropertyDescriber(ClassDescriber<T> parent, Func<T, object> func)
                {
                    _parent = parent;
                    _func = func;
                }
                public IClassDescriber<T> Is(string url)
                {
                    _parent._urls.Add(_func, url);
                    return _parent;
                }
            }

            private class CollectionDescriber : ICollectionDescriber<T>
            {
                private readonly ClassDescriber<T> _parent;
                private readonly Func<T, object> _func;

                public CollectionDescriber(ClassDescriber<T> parent, Func<T, object> func)
                {
                    _parent = parent;
                    _func = func;
                }

                public IClassDescriber<T> Are(string url)
                {
                    _parent._urls.Add(_func, url);
                    return _parent;
                }
            }
        }
    }

    public interface IClassDescriber<T> where T : class
    {
        IPropertyDescriber<T> Property(Func<T, string> func);
        IPropertyDescriber<T> Property(Func<T, object> func);
        ICollectionDescriber<T> Property<P>(Func<T, IEnumerable<P>> func);
    }



    public interface IPropertyDescriber<T> where T : class
    {
        IClassDescriber<T> Is(string url);
    }

    public interface ICollectionDescriber<T> where T : class
    {
        IClassDescriber<T> Are(string url);
    }
}
