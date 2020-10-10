using System;
using System.Drawing;

namespace KEUtils {
    class RainbowColorScheme {
        private static readonly int NCOLORS = 256;
        private int nColors = NCOLORS;
        private Color[] colors = null;

        /**
         * RainbowColorScheme default constructor (256 colors).
         */
        public RainbowColorScheme() {
        }

        /**
         * RainbowColorScheme constructor.
         * 
         * @param nColors The number of colors in the scheme.
         */
        public RainbowColorScheme(int nColors) {
            this.nColors = nColors;
        }

        /**
         * Defines the Color array.
         */
        public Color[] defineColors() {
            if (colors != null) return colors;

            colors = new Color[nColors];
            // Make a rainbow palette
            for (int i = 0; i < nColors; i++) {
                colors[i] = defineColor(i, nColors);
            }
            return colors;
        }

        /**
         * Gets a color corresponding to a given number of colors.
         * 
         * @param index Index of the color [0, nColors - 1].
         * @param nColors The total number of colors.
         * @return The Color corresponding to the index.
         */
        public static Color defineColor(int index, int nColors) {
            Color color;
            double nGroups = 5, nMembers = 45, nTotal = nGroups * nMembers;
            double high = 1.000, medium = .375;

            double h = (double)index / (double)nColors;
            double hx = h * nTotal;
            double deltax = (high - medium) / nMembers;
            double r, g, b;
            int gh = (int)Math.Floor(hx / nMembers);
            int ih = (int)Math.Floor(hx);
            switch (gh) {
                case 0:
                    r = medium;
                    g = medium + (ih - gh * nMembers) * deltax;
                    b = high;
                    break;
                case 1:
                    r = medium;
                    g = high;
                    b = high - (ih - gh * nMembers) * deltax;
                    break;
                case 2:
                    r = medium + (ih - gh * nMembers) * deltax;
                    g = high;
                    b = medium;
                    break;
                case 3:
                    r = high;
                    g = high - (ih - gh * nMembers) * deltax;
                    b = medium;
                    break;
                case 4:
                    r = high;
                    g = medium;
                    b = medium + (ih - gh * nMembers) * deltax;
                    break;
                default:
                    r = high;
                    g = medium;
                    b = high;
                    break;
            }
            int red = (int)(r * 255 + .5);
            if (red > 255) red = 255;
            int green = (int)(g * 255 + .5);
            if (green > 255) green = 255;
            int blue = (int)(b * 255 + .5);
            if (blue > 255) blue = 255;
            color = Color.FromArgb(red, green, blue);
            return color;
        }

        /**
         * Calculates the integer value of the Color.
         * 
         * @param color
         * @return 256*256*red + 256*green + blue.
         */
        public static int toColorInt(Color color) {
            int colorInt = 65536 * color.R + 256 * color.G + color.B;
            return colorInt;
        }

        /**
         * Calculates the string value of the Color.
         * 
         * @param color
         * @return "rrr,ggg,bbb".
         */
        public static String toColorString(Color color) {
            String str = color.R + "," + color.G + "," + color.B;
            return str;
        }

        /**
         * Calculates the color corresponding to a fraction of the default number of
         * colors (256).
         * 
         * @param fract A fraction in the range [0,1] inclusive.
         * @return The Color with index closest to the fraction times the maximun
         *         color index (255).
         */
        public static Color getColor(double fract) {
            return getColor(fract, NCOLORS);
        }

        /**
         * Returns the color corresponding to a fraction of the number of colors
         * from the stored color array. Calculates the array if it has not
         * previously been calculated.
         * 
         * @param fract A fraction in the range [0,1] inclusive.
         * @param nColors The total number of colors.
         * @return The Color with index closest to the fraction times the maximun
         *         color index (nColors - 1).
         */
        public Color getStoredColor(double fract) {
            if (colors == null) defineColors();
            // This shouldn't happen and it might be better to return Color.Black
            //if (colors == null) return Color.Empty;
            if (colors == null) return Color.Black;
            int index = (int)Math.Round((nColors - 1) * fract);
            if (index < 0) index = 0;
            if (index > nColors) index = nColors;
            return colors[index];
        }

        /**
         * Calculates the color corresponding to a fraction of the number of colors.
         * 
         * @param fract A fraction in the range [0,1] inclusive.
         * @param nColors The total number of colors.
         * @return The Color with index closest to the fraction times the maximun
         *         color index (nColors - 1).
         */
        public static Color getColor(double fract, int nColors) {
            int index = (int)Math.Round((nColors - 1) * fract);
            if (index < 0) index = 0;
            if (index > nColors) index = nColors;
            return defineColor(index, nColors);
        }


    }
}
