using System;

namespace SolarSwing
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SolarSwing game = new SolarSwing())
            {
                game.Run();
            }
        }
    }
#endif
}

