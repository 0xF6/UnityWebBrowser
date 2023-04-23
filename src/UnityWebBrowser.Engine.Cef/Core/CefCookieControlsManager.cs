// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Collections.Generic;
using System.CommandLine.Completions;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using VoltstroStudios.UnityWebBrowser.Shared.Core;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Core;

public class CefCookieControlsManager : ICookieControls
{
    public void Init()
    {
    }

    public UwbCookie Get(string url, string key)
    {
        var visitor = new CefCookieAccessor();
        var result = visitor.GetCookie(url, key);

        result.RunSynchronously();

        return result.Result;
    }

    public bool Set(string url, UwbCookie cookie)
    {
        var visitor = new CefCookieAccessor();
        var result = visitor.SetCookie(url, cookie.Path, cookie);

        result.RunSynchronously();

        return result.Result;
    }

    public void Delete(string url, string key)
    {
        var visitor = new CefCookieAccessor();
        var result = visitor.DeleteCookie(url, key);
        result.RunSynchronously();
    }

    public class CefCookieAccessor : CefCookieVisitor
    {
        public List<UwbCookie> Cookies = new ();

        protected override bool Visit(CefCookie cookie, int count, int total, out bool delete)
        {
            Cookies.Add(new UwbCookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));

            delete = false;
            return true;
        }


        public Task<List<UwbCookie>> GetCookies(string address)
        {
            var completionSource = new TaskCompletionSource<List<UwbCookie>>();
            getGlobalManager(cookieManager =>
            {
                cookieManager.VisitUrlCookies(address, false, this);
                completionSource.SetResult(Cookies);
            }, e => completionSource.SetException(e));
            return completionSource.Task;
        }

        public Task<UwbCookie> GetCookie(string address, string key)
        {
            var completionSource = new TaskCompletionSource<UwbCookie>();
            getGlobalManager(cookieManager =>
            {
                cookieManager.VisitUrlCookies(address, false, this);
                var cookie = Cookies.First(x => x.Name.Equals(key));
                completionSource.SetResult(cookie);
            }, e => completionSource.SetException(e));
            return completionSource.Task;
        }

        public Task DeleteCookie(string address, string key)
        {
            var completionSource = new TaskCompletionSource();
            getGlobalManager(cookieManager =>
            {
                cookieManager.DeleteCookies(address, key, new DeleteCallback(completionSource.SetResult));
            }, e => completionSource.SetException(e));
            return completionSource.Task;
        }

        public Task<bool> SetCookie(string address, string key, UwbCookie cookie)
        {
            var completionSource = new TaskCompletionSource<bool>();
            getGlobalManager(cookieManager => {
                cookieManager.SetCookie(address, new CefCookie()
                {
                    Path = cookie.Path,
                    Name = cookie.Name,
                    Value = cookie.Value,
                    Domain = cookie.Domain,
                    HttpOnly = false,
                    Secure = false,
                }, new SetCallback(completionSource.SetResult));
            }, e => completionSource.SetException(e));
            return completionSource.Task;
        }


        private static void getGlobalManager(Action<CefCookieManager> action, Action<Exception> fail)
        {
            try
            {
                CefCookieManager manager = null;
                manager = CefCookieManager.GetGlobal(new CompletionCallback(() => {
                    action?.Invoke(manager);
                }));
            }
            catch (Exception e)
            {
                fail(e);
            }
        }

        private class CompletionCallback : CefCompletionCallback
        {
            private readonly Action actor;

            public CompletionCallback(Action actor) 
                => this.actor = actor;

            protected override void OnComplete()
                => actor.Invoke();
        }

        private class DeleteCallback : CefDeleteCookiesCallback
        {
            private readonly Action actor;

            public DeleteCallback(Action actor)
                => this.actor = actor;

            protected override void OnComplete(int numDeleted)
                => actor.Invoke();
        }

        private class SetCallback : CefSetCookieCallback
        {
            private readonly Action<bool> actor;

            public SetCallback(Action<bool> actor)
                => this.actor = actor;

            protected override void OnComplete(bool success)
                => actor.Invoke(success);
        
        }
}
}