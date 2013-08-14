using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Data.Tests
{
    public static class TestHelper
    {
        public static bool RandomizeBoolean()
        {
            var randomNumber = new Random().Next(0, 2);
            return randomNumber == 1;
        }

        public static int RandomizeInt()
        {
            return RandomizeNumber();
        }

        public static string RandomizeString()
        {
            return RandomizeString(string.Empty);
        }

        public static string RandomizeString(string prefixion)
        {
            if (null == prefixion)
                prefixion = string.Empty;

            return string.Concat(prefixion, RandomizeNumber());
        }

        private static int RandomizeNumber()
        {
            Thread.Sleep(100);

            var number = new Random().Next(1000, 9000);
            return number;
        }

    }
}
