using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions;

public class AnimatedPropertyRegistration : IDisposable
{
	/// <summary>
	/// Name of the property
	/// </summary>
	public string FullName => _fullName;
	/// <summary>
	/// Name of the property wrapped in "var(...)". Ready to be applied to css.
	/// </summary>
	public string InvokableName => $"var({_fullName})";

	public void Pause()
		=> _updateIsRunnning(false);

	public void Resume()
		=> _updateIsRunnning(true);



	private readonly Action _unregister;
	private readonly Action<bool> _updateIsRunnning;
	private readonly string _fullName;

	internal AnimatedPropertyRegistration(
		Action unregister,
		Action<bool> updateIsRunning,
		string nameWithoutPrefix)
	{
		_unregister = unregister;
		_updateIsRunnning = updateIsRunning;
		_fullName = "--" + nameWithoutPrefix;
	}

	public void Dispose()
	{
		_unregister();
	}
}
