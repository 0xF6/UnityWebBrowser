// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using VoltRpc.IO;
using VoltRpc.Proxy;
using VoltRpc.Types;

namespace VoltstroStudios.UnityWebBrowser.Shared.Core;

/// <summary>
///     Shared interface for events that client has
/// </summary>
[GenerateProxy(GeneratedName = "CookieControls", GeneratedNamespace = "VoltstroStudios.UnityWebBrowser.Shared.Core")]
internal interface ICookieControls
{
    /// <summary>
    ///     Pull Cookie value by key
    /// </summary>
    /// <param name="url">address</param>
    /// <param name="key">Cookie key</param>
    public UwbCookie Get(string url, string key);
    /// <summary>
    ///     Set Cookie value by key
    /// </summary>
    /// <param name="url">address</param>
    /// <param name="cookie">Cookie</param>
    public bool Set(string url, UwbCookie cookie);

    /// <summary>
    ///     Delete Cookie value by key
    /// </summary>
    /// <param name="url">address</param>
    /// <param name="key">Cookie key</param>
    public void Delete(string url, string key);
}

/// <summary>
///     Entity of cookie
/// </summary>
public class UwbCookie
{
    /// <summary>
    ///     Empty ctor
    /// </summary>
    public UwbCookie() { }
    
    /// <summary>
    ///     create instance with all fields
    /// </summary>
    public UwbCookie(string name, string value, string path, string domain)
    {
        this.Name = name; 
        this.Value = value;
        this.Path = path;
        this.Domain = domain;
    }

    /// <summary>
    ///     The cookie name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     The cookie value.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// If |domain| is empty a host cookie will be created instead of a domain
    /// cookie. Domain cookies are stored with a leading "." and are visible to
    /// sub-domains whereas host cookies are not.
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// If |path| is non-empty only URLs at or below the path will get the cookie
    /// value.
    /// </summary>
    public string Path { get; set; }
}


public class UwbCookieReaderWriter : TypeReadWriter<UwbCookie>
{
    public override void Write(BufferedWriter writer, UwbCookie value)
    {
        writer.WriteString(value.Name);
        writer.WriteString(value.Value);
        writer.WriteString(value.Domain);
        writer.WriteString(value.Path);
    }

    public override UwbCookie Read(BufferedReader reader) => new()
    {
        Name = reader.ReadString(),
        Value = reader.ReadString(),
        Domain = reader.ReadString(),
        Path = reader.ReadString()
    };
}