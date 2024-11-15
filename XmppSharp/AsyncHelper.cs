using System.Globalization;

namespace XmppSharp;

public static class AsyncHelper
{
    static readonly TaskFactory s_taskFactory = new(
        CancellationToken.None,
        TaskCreationOptions.None,
        TaskContinuationOptions.None,
        TaskScheduler.Default
    );

    static (CultureInfo, CultureInfo) GetCurrentCulture()
        => (CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture);

    static void SetCurrentCulture((CultureInfo, CultureInfo) v)
        => (CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture) = v;

    public static Task RunAsync(Action callback)
    {
        ThrowHelper.ThrowIfNull(callback);

        var culture = GetCurrentCulture();

        return s_taskFactory.StartNew(() =>
        {
            SetCurrentCulture(culture);
            callback();
        });
    }

    public static Task RunAsync(Action<object?> callback, object? argument)
    {
        ThrowHelper.ThrowIfNull(callback);

        var culture = GetCurrentCulture();

        return s_taskFactory.StartNew(() =>
        {
            SetCurrentCulture(culture);
            callback(argument);
        });
    }

    public static Task RunAsync<TArgument>(Action<TArgument?> callback, TArgument? argument)
    {
        ThrowHelper.ThrowIfNull(callback);

        var culture = GetCurrentCulture();

        return s_taskFactory.StartNew(() =>
        {
            SetCurrentCulture(culture);
            callback(argument);
        });
    }

    public static void RunSync(Func<Task> task)
    {
        ThrowHelper.ThrowIfNull(task);

        var culture = GetCurrentCulture();

        s_taskFactory.StartNew(() =>
        {
            SetCurrentCulture(culture);
            return task();
        }).Unwrap().GetAwaiter().GetResult();
    }

    public static TResult RunSync<TResult>(Func<Task<TResult>> task)
    {
        ThrowHelper.ThrowIfNull(task);

        var culture = GetCurrentCulture();

        return s_taskFactory.StartNew(() =>
         {
             SetCurrentCulture(culture);
             return task();
         }).Unwrap().GetAwaiter().GetResult();
    }
}
