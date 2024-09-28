using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssAnimations.JsInterop;

internal record DOMRect(
    double X,
    double Y,
    double Width,
    double Height,
    double Top,
    double Right,
    double Bottom,
    double Left);
