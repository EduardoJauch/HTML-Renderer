﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;


namespace TheArtOfDev.HtmlRenderer.Core
{
    public interface IResourceServer: IDisposable
    {
        void SetHtml(string html);
        Task<String> GetHtmlAsync();

        void SetCssData(CssData cssData);
        Task<CssData> GetCssDataAsync();

        Task<CssData> GetCssDataAsync(string location, Dictionary<string, string> attributes);
        Task<RImage> GetImageAsync(string location, Dictionary<string, string> attributes);
    }

    public class DefaultResourceServer : IResourceServer
    {
        public void Dispose()
        {
        }

        string m_html;
        public void SetHtml(string html)
        {
            m_html = html;
        }
        public async Task<string> GetHtmlAsync()
        {
            return m_html;
        }

        CssData m_cssData;
        public void SetCssData(CssData cssData)
        {
            m_cssData = cssData;
        }
        public async Task<CssData> GetCssDataAsync()
        {
            return m_cssData;
        }

        public Task<CssData> GetCssDataAsync(string location, Dictionary<string, string> attributes)
        {
            throw new NotImplementedException();
        }

        public Task<RImage> GetImageAsync(string location, Dictionary<string, string> attributes)
        {
            throw new NotImplementedException();
        }
    }

    public static class ResourceServerFactory
    {
        public delegate IResourceServer Factory();

        static Factory s_func = () => new DefaultResourceServer();

        public static void Iniialize(Factory func)
        {
            s_func = func;
        }

        public static IResourceServer Create()
        {
            return s_func();}
    }
}