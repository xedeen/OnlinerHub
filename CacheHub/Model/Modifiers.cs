using System;

namespace CacheHub.Model
{
    public enum TagSize
    {
        Small = 0,
        Big = 1
    }

    [Flags]
    public enum TextModifiers
    {
        None = 0x0000,
        Bold = 0x0001,
        Italic = 0x0010,
        Underline = 0x0100
    }
}
