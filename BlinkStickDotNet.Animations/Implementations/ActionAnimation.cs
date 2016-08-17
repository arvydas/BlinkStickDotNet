using BlinkStickDotNet.Animations.Parallel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlinkStickDotNet.Animations.Implementations
{
    public class ActionAnimation : IAnimation
    {
        Action<IParallelProcessor> _action;

        public ActionAnimation(Action<IParallelProcessor> action)
        {
            _action = action;
        }

        public IAnimation Clone()
        {
            return new ActionAnimation(_action);
        }

        public void Start(IBlinkStickColorProcessor processor)
        {
            _action(new Processor(processor));
        }

        class Processor : IParallelProcessor
        {
            IBlinkStickColorProcessor _stick;

            public Processor(IBlinkStickColorProcessor stick)
            {
                _stick = stick;

                uint i = 0;

                this.Leds = stick
                    .GetCurrentColors()
                    .CloneArray((int)_stick.NrOfLeds)
                    .Select(c => new Led(i++, c))
                    .ToArray();

            }

            public Led[] Leds
            {
                get;
                private set;
            }

            public void Process()
            {
                var colors = this.Leds
                    .OrderBy(l => l.LedNr)
                    .Select(l => l.Color)
                    .ToArray();

                _stick.ProcessColors(colors);
            }
        }
    }
}