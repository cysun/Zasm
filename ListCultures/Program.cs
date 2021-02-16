using System;
using System.Globalization;

namespace ListCultures
{
    class Program
    {
        static void Main()
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var culture in cultures)
                Console.WriteLine($"{culture.Name} {culture.EnglishName}");
        }
    }
}
