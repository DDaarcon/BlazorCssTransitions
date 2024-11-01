using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedProperties;

public interface IAnimatedPropertyRegistration : IDisposable
{
	string FullName { get; }
	string InvokableName { get; }

	void Pause();
	void Resume();
}
