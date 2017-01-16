using System;
using System.Collections.Generic;
using System.Linq;

namespace TwitterStream
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TwitterStream stream = new TwitterStream();
                stream.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
