using System;

namespace Overlord.Other
{
    public class SimpleThing : ISimpleThing
    {
        public void DoStuff()
        {
            Console.Write("Hello world");
        }
    }

    public interface ISimpleThing
    {
        void DoStuff();
    }
}
