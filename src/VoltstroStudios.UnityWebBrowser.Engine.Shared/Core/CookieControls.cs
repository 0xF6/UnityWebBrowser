// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Net;
using VoltRpc.Communication;
using VoltstroStudios.UnityWebBrowser.Shared.Core;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;

public class CookieControlsActions : ICookieControls, IDisposable
{
    private Client? client;
    private ICookieControls? clientCookieManager;
    
    #region IDisposable

    ~CookieControlsActions()
    {
        ReleaseResources();
    }

    private void ReleaseResources()
    {
        client?.Dispose();
    }

    public void Dispose()
    {
        ReleaseResources();
        GC.SuppressFinalize(this);
    }

    #endregion




    internal void SetIpcClient(Client ipcClient)
    {
        client = ipcClient ?? throw new NullReferenceException();
        clientCookieManager = new CookieControls(client);
    }

    public UwbCookie Get(string url, string key)
    {
        if (client is { IsConnected: true } && clientCookieManager is { })
            return clientCookieManager.Get(url, key);
        return null;
    }

    public bool Set(string url, UwbCookie cookie)
    {
        if (client is { IsConnected: true } && clientCookieManager is { })
            return clientCookieManager.Set(url, cookie);
        return false;
    }

    public void Delete(string url, string key)
    {
        if (client is { IsConnected: true } && clientCookieManager is { })
            clientCookieManager.Delete(url, key);
    }
}