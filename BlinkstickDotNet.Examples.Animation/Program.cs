using BlinkStickDotNet;
using BlinkStickDotNet.Animations;
using BlinkStickDotNet.Animations.Implementations;
using System;
using System.Drawing;

namespace BlinkstickDotNet.Examples.Animation
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var blue = Color.Blue.Darken(0.6);
            var orange = Color.FromArgb(255, 75, 0);
            var green = Color.FromArgb(0, 70, 0);

            var stick = BlinkStick.FindFirst();
            if (stick.OpenDevice())
            {
                var queue = new AnimationQueue(true);

                queue.Queue(new Feedback("Morph to red 5s"));
                queue.Morph(5000, Color.Red);

                queue.Queue(new Feedback("Morph to green 5s"));
                queue.Morph(5000, Color.Green);

                queue.Queue(new Feedback("Morph to blue 5s"));
                queue.Morph(5000, Color.Blue);

                queue.Queue(new Feedback("Morph to red 5s"));
                queue.Morph(5000, Color.Red);
                
                queue.Queue(new Feedback("Pulse red 1s"));
                queue.Pulse(1000, Color.Red);

                queue.Queue(new Feedback("Pulse green 1s"));
                queue.Pulse(1000, green);

                queue.Queue(new Feedback("Pulse blue 1s"));
                queue.Pulse(1000, Color.Blue);

                queue.Queue(new Feedback("Chase 1 orange led for 1000 for 8 times"));
                queue.Chase(1000, orange.PadBlack(8));
                queue.Repeat(8);

                queue.Queue(new Feedback("Off for 2000"));
                queue.Off(2000);

                queue.Queue(new Feedback("Halve green for 2000"));
                queue.Color(2000, green.PadBlack(2));

                queue.Queue(new Feedback("Full green for 2000"));
                queue.Color(2000, green);

                queue.Queue(new Feedback("Ready!"));

                queue.Connect(stick, 8);
                queue.Start();

                try
                {
                    Console.ReadLine();
                }
                finally
                {
                    queue.Stop(true);
                }
            }
        }
    }
}