// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace Windows.Win32.Foundation;

internal static class HwndExtensions
{
    /// <summary>
    ///  Returns <see langword="true"/> if <paramref name="value"/> matches the class name of <paramref name="hwnd"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   <see href="https://devblogs.microsoft.com/oldnewthing/20101231-00/?p=11863">
    ///    What makes RealGetWindowClass so much more real than GetClassName?
    ///   </see>
    ///  </para>
    /// </remarks>
    [SkipLocalsInit]
    internal unsafe static bool RealClassNameEquals(this HWND hwnd, ReadOnlySpan<char> value, StringComparison comparison = StringComparison.Ordinal)
    {
        if (hwnd.IsNull)
        {
            return false;
        }

        Span<char> buffer = stackalloc char[PInvokeCore.MaxClassName];
        uint length = 0;
        fixed (char* lpClassName = buffer)
        {
            length = PInvoke.RealGetWindowClass(hwnd, lpClassName, (uint)buffer.Length);
            return length != 0 && value.Equals(buffer[..(int)length], comparison);
        }
    }

    /// <summary>
    ///  Returns <see langword="true"/> if <paramref name="value"/> matches the class name of <paramref name="hwnd"/>.
    /// </summary>
    [SkipLocalsInit]
    internal unsafe static bool ClassNameEquals(this HWND hwnd, ReadOnlySpan<char> value, StringComparison comparison = StringComparison.Ordinal)
    {
        if (hwnd.IsNull)
        {
            return false;
        }

        Span<char> buffer = stackalloc char[PInvokeCore.MaxClassName];
        int length = 0;
        fixed (char* lpClassName = buffer)
        {
            length = PInvoke.GetClassName(hwnd, lpClassName, buffer.Length);
            return length != 0 && value.Equals(buffer[..length], comparison);
        }
    }

    /// <summary>
    ///  Returns <see langword="true"/> if any <paramref name="values"/> matches the class name of <paramref name="hwnd"/>.
    /// </summary>
    [SkipLocalsInit]
    internal unsafe static bool ClassNameEqualsAny(this HWND hwnd, StringComparison comparison, params ReadOnlySpan<string> values)
    {
        if (hwnd.IsNull)
        {
            return false;
        }

        Span<char> buffer = stackalloc char[PInvokeCore.MaxClassName];
        int length = 0;
        fixed (char* lpClassName = buffer)
        {
            length = PInvoke.GetClassName(hwnd, lpClassName, buffer.Length);
            buffer = buffer[..length];

            foreach (string value in values)
            {
                if (length != 0 && value.AsSpan().Equals(buffer, comparison))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    ///  Returns <see langword="true"/> if <paramref name="value"/> is contained in the class name of <paramref name="hwnd"/>.
    /// </summary>
    [SkipLocalsInit]
    internal unsafe static bool ClassNameContains(this HWND hwnd, ReadOnlySpan<char> value, StringComparison comparison = StringComparison.Ordinal)
    {
        if (hwnd.IsNull)
        {
            return false;
        }

        ReadOnlySpan<char> buffer = stackalloc char[PInvokeCore.MaxClassName];
        int length = 0;
        fixed (char* lpClassName = buffer)
        {
            length = PInvoke.GetClassName(hwnd, lpClassName, buffer.Length);
            return length != 0 && buffer[..length].IndexOf(value, comparison) != -1;
        }
    }
}
