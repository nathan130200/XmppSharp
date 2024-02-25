using System.Diagnostics;

namespace XmppSharp;

public delegate Task AsyncEventHandler();
public delegate Task<R> AsyncEventHandler<R>();
public delegate Task ParameterizedAsyncEventHandler<P>(P param);
public delegate Task<R> ParameterizedAsyncEventHandler<P, R>(P param);

public static class AsyncEventHandlerUtil
{
    public static async Task InvokeAsync(this AsyncEventHandler handler)
    {
        try
        {
            if (handler != null)
                await handler();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    public static async Task<R?> InvokeAsync<R>(this AsyncEventHandler<R> handler)
    {
        try
        {
            if (handler != null)
                return await handler();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

        return default;
    }

    public static async Task InvokeAsync<P>(this ParameterizedAsyncEventHandler<P> handler, P param)
    {
        try
        {
            if (handler != null)
                await handler(param);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    public static async Task<R?> InvokeAsync<P, R>(this ParameterizedAsyncEventHandler<P, R> handler, P param)
    {
        try
        {
            if (handler != null)
                return await handler(param);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

        return default;
    }
}