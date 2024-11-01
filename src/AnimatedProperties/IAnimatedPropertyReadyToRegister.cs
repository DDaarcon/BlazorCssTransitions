using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedProperties;

public interface IAnimatedPropertyReadyToRegister<TRegistration>
{
	/// <summary>
	/// Registers property in provider (renders it on the page).
	/// </summary>
	/// <returns>
	/// A disposable property registration.
	/// </returns>
	TRegistration Create();
}
