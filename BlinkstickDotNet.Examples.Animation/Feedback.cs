using System;
using BlinkStickDotNet.Animations;
using BlinkStickDotNet.Animations.Processors;

namespace BlinkstickDotNet.Examples.Animation
{
    public class Feedback : IAnimation
    {
        private string _message;

        public Feedback(string message)
        {
            _message = message;
        }

        public IAnimation Clone()
        {
            return new Feedback(_message);
        }

        public void Start(ILedProcessor processor)
        {
            Console.WriteLine(_message);
        }

        public void Start(IColorProcessor processor)
        {
            Console.WriteLine(_message);
        }
    }
}
