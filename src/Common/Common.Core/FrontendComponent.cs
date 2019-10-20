using System;
using System.Collections.Generic;

namespace Common.Core
{
    public abstract class FrontendComponent
    {
        protected FrontendComponent(string hubUrl)
        {
            Url = hubUrl ?? throw new ArgumentNullException(nameof(hubUrl));
        }

        public string Url { get; }

        public static FrontendComponent<T> Create<T>(T data, string componentUrl, string hubUrl = null) where T : class
        {
            return !string.IsNullOrWhiteSpace(hubUrl)
                ? new RealtimeFrontendComponent<T>(componentUrl, hubUrl, data)
                : new FrontendComponent<T>(componentUrl, data);
        }
    }

    public class FrontendComponent<T> : FrontendComponent
    {
        internal FrontendComponent(string componentUrl, T data): base(componentUrl)
        {
            Data = data;
        }

        public T Data { get; }
    }

    public class RealtimeFrontendComponent<T> : FrontendComponent<T>
    {
        internal RealtimeFrontendComponent(string componentUrl, string hubUrl, T data): base(componentUrl, data)
        {
            Hub = hubUrl;
        }

        public string Hub { get; }
    }
}
