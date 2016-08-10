using System;
using BlinkStickDotNet.Animations;

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

        public void Start(IBlinkStickColorProcessor processor)
        {
            Console.WriteLine(_message);
        }
    }
}
