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
        Throw.IfNull(callback);

        var culture = GetCurrentCulture();

        return s_taskFactory.StartNew(() =>
        {
            SetCurrentCulture(culture);
            callback();
        });
    }

    public static Task RunAsync(Action<object?> callback, object? argument)
    {
        Throw.IfNull(callback);

        var culture = GetCurrentCulture();

        return s_taskFactory.StartNew(() =>
        {
            SetCurrentCulture(culture);
            callback(argument);
        });
    }

    public static Task RunAsync<TArgument>(Action<TArgument?> callback, TArgument? argument)
    {
        Throw.IfNull(callback);

        var culture = GetCurrentCulture();

        return s_taskFactory.StartNew(() =>
        {
            SetCurrentCulture(culture);
            callback(argument);
        });
    }

    public static void RunSync(Func<Task> task)
    {
        Throw.IfNull(task);

        var culture = GetCurrentCulture();

        s_taskFactory.StartNew(() =>
        {
            SetCurrentCulture(culture);
            return task();
        }).Unwrap().GetAwaiter().GetResult();
    }

    public static TResult RunSync<TResult>(Func<Task<TResult>> task)
    {
        Throw.IfNull(task);

        var culture = GetCurrentCulture();

        return s_taskFactory.StartNew(() =>
         {
             SetCurrentCulture(culture);
             return task();
         }).Unwrap().GetAwaiter().GetResult();
    }
}
