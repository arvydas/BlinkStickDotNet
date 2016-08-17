using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlinkStickDotNet.Animations.Parallel
{
    public interface IParallelProcessor
    {
        Led[] Leds { get; }

        void Process();
    }
}
