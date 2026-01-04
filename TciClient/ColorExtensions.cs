using System.Drawing;

namespace ExpertElectronics.Tci
{
    /// <summary>
    /// Extension helpers for converting <see cref="Color"/> instances to string representations.
    /// </summary>
    public static class ColorConverterExtensions
    {
        /// <summary>
        /// Converts a <see cref="Color"/> to a hexadecimal color string (#RRGGBB).
        /// </summary>
        /// <param name="c">The <see cref="Color"/> to convert.</param>
        /// <returns>A string in the form #RRGGBB.</returns>
        public static string ToHexString(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        /// <summary>
        /// Converts a <see cref="Color"/> to an RGB textual representation.
        /// </summary>
        /// <param name="c">The <see cref="Color"/> to convert.</param>
        /// <returns>A string in the form "RGB(r, g, b)".</returns>
        public static string ToRgbString(this Color c) => $"RGB({c.R}, {c.G}, {c.B})";
    }
}
