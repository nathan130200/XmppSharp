using System.Diagnostics;

namespace XmppSharp;

public delegate Task AsyncAction();
public delegate Task AsyncAction<TParam>(TParam param);

public delegate Task<TResult> AsyncFunc<TResult>();
public delegate Task<TResult> AsyncFunc<TResult, TParam>(TParam param);

public static class AsyncUtilities
{
    public static async Task InvokeAsync(this AsyncAction func)
    {
        try
        {
            if (func != null)
                await func();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    public static async Task InvokeAsync<TParam>(this AsyncAction<TParam> func, TParam param)
    {
        try
        {
            if (func != null)
                await func(param);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    public static async Task<TResult> InvokeAsync<TResult>(this AsyncFunc<TResult> func)
    {
        try
        {
            if (func != null)
                return await func();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

        return default;
    }

    public static async Task<TResult> InvokeAsync<TResult, TParam>(this AsyncFunc<TResult, TParam> func, TParam param)
    {
        try
        {
            if (func != null)
                return await func(param);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

        return default;
    }
}