using System;
using BlinkStickDotNet.Animations;

namespace BlinkstickDotNet.Examples.Animation
{
    public class ConsoleAnimation : IAnimation
    {
        private string _message;

        public ConsoleAnimation(string message)
        {
            _message = message;
        }

        public IAnimation Clone()
        {
            return new ConsoleAnimation(_message);
        }

        public void Start(IBlinkStickColorProcessor processor)
        {
            Console.WriteLine(_message);
        }
    }
}
