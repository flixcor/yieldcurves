using System;

namespace Common.Core
{
    public abstract class FrontendComponent
    {
        protected FrontendComponent(string url)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public string Url { get; }

        public static FrontendComponent<T> Create<T>(T data, string url)
        {
            return new FrontendComponent<T>(url, data);
        }
    }

    public class FrontendComponent<T> : FrontendComponent
    {
        internal FrontendComponent(string url, T data): base(url)
        {
            Data = data;
        }

        public T Data { get; }
    }
}
