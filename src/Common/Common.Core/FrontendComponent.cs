using System;
using System.Collections.Generic;

namespace Common.Core
{
    public abstract class FrontendComponent
    {
        protected FrontendComponent(string url)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public string Url { get; }

        public static FrontendComponent<T> Create<T>(T data, string url) where T : class
        {
            return new FrontendComponent<T>(url, data);
        }

        public static RealtimeFrontendComponent<T> Create<T>(T data, string url, string hub) where T : class
        {
            return new RealtimeFrontendComponent<T>(url, hub, data);
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

    public class RealtimeFrontendComponent<T> : FrontendComponent<T>
    {
        internal RealtimeFrontendComponent(string url, string hub, T data): base(url, data)
        {
            Hub = hub;
        }

        public string Hub { get; }
    }
}
