using System.Diagnostics;

namespace XmppSharp;

/// <summary>
/// Type that represents a delegate for an asynchronous event without parameters.
/// </summary>
/// <returns>Task that asynchronously invokes the event.</returns>
public delegate Task AsyncAction();

/// <summary>
/// Type that represents a delegate for an asynchronous event that has a parameter.
/// </summary>
/// <typeparam name="TParam">Parameter type for the event.</typeparam>
/// <param name="param">Parameter value.</param>
/// <returns>Task that asynchronously invokes the event.</returns>
public delegate Task AsyncAction<TParam>(TParam param);

/// <summary>
/// Type that represents the delegate of an asynchronous event that returns a value.
/// </summary>
/// <typeparam name="TResult">Type of result that will be returned.</typeparam>
/// <returns>Task that asynchronously invokes the event and returns the result.</returns>
public delegate Task<TResult> AsyncFunc<TResult>();

/// <summary>
/// Type that represents the delegate of an asynchronous event that has a parameter and returns a value.
/// </summary>
/// <typeparam name="TResult">Type of result that will be returned.</typeparam>
/// <typeparam name="TParam">Parameter type for the event.</typeparam>
/// <param name="param"></param>
/// <returns>Task that asynchronously invokes the event and returns the result.</returns>
public delegate Task<TResult> AsyncFunc<TResult, TParam>(TParam param);

public static class AsyncUtilities
{
	/// <summary>
	/// Helper function that safely invokes the event.
	/// </summary>
	/// <param name="func">Delegate of the event that will be invoked.</param>
	/// <returns>Returns a task that invokes the event asynchronously.</returns>
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

	/// <summary>
	/// Helper function that safely invokes the event.
	/// </summary>
	/// <typeparam name="TParam"> Parameter type for the event.</typeparam>
	/// <param name="func">Delegate of the event that will be invoked.</param>
	/// <param name="param">Parameter value.</param>
	/// <returns>Returns a task that invokes the event asynchronously.</returns>
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

	/// <summary>
	/// Helper function that safely invokes the event and returns a value.
	/// </summary>
	/// <typeparam name="TResult">Type of result that will be returned.</typeparam>
	/// <returns>When awaited, returns the event value or the default value of <typeparamref name="TResult"/> if an error occurs.</returns>
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

	/// <summary>
	/// Helper function that safely invokes the event.
	/// </summary>
	/// <typeparam name="TParam">Parameter type for the event.</typeparam>
	/// <typeparam name="TResult">Type of result that will be returned.</typeparam>
	/// <param name="func">Delegate of the event that will be invoked.</param>
	/// <param name="param">Parameter value.</param>
	/// <returns>When awaited, returns the event value or the default value of <typeparamref name="TResult" /> if an error occurs.
	/// <para>The error will be displayed in the debug window using <see cref="Debug.WriteLine(object)" />.</para>
	/// </returns>
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