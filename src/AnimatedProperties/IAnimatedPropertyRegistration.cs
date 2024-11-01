using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedProperties;

public interface IAnimatedPropertyRegistration : IDisposable
{
    /// <summary>
    /// Name of the property. The name is generated out of GUID, so should be unique.
    /// </summary>
    string FullName { get; }
    /// <summary>
    /// Name of the property wrapped in "var(...)". Ready to be applied to css.
    /// </summary>
    string InvokableName { get; }

    void Pause();
	void Resume();
}
