using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulldog3.HelperClasses
{
    /// <summary>
    /// Class used to remap values to colors
    /// </summary>
    public static class ColorRemapper
    {
        /// <summary>
        /// Remap a specific value to a color given the boundary conditions
        /// </summary>
        /// <param name="inFirstColor"></param>
        /// <param name="inSecondColor"></param>
        /// <param name="maxVal"></param>
        /// <param name="minVal"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color RemappedColor(ref Color inFirstColor, ref Color inSecondColor, double maxVal, double minVal, double value)
        {
            int alpha = (int)Remapper.Map(value, minVal, maxVal, inFirstColor.A, inSecondColor.A);
            int red = (int)Remapper.Map(value, minVal, maxVal, inFirstColor.R, inSecondColor.R);
            int green = (int)Remapper.Map(value, minVal, maxVal, inFirstColor.G, inSecondColor.G);
            int blue = (int)Remapper.Map(value, minVal, maxVal, inFirstColor.B, inSecondColor.B);

            Color color = Color.FromArgb(alpha, red, green, blue);
            return color;
        }
    }
}
