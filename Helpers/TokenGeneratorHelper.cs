using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kiosk.Helpers;
public static class TokenGeneratorHelper
{
    public static string Generate(string prefix, int number)
    {
        return $"{prefix}-{number:D4}";
    }
}