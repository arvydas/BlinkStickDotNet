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
            var device = BlinkStick.FindFirst();
            if (device.OpenDevice())
            {
                var queue = device.CreateAnimationQueue(true);

                queue.Color(2000, Color.Blue);
                queue.Queue(new PulseInvertedAnimation(2000));

                /*
                queue.Queue(new Feedback("Morph to red 5s"));
                queue.Morph(5000, Color.Red);

                queue.Queue(new Feedback("Morph to green 5s"));
                queue.Morph(5000, Color.Green);

                queue.Queue(new Feedback("Morph to blue 5s"));
                queue.Morph(5000, Color.Blue);

                queue.Queue(new Feedback("Morph to red 5s"));
                queue.Morph(5000, Color.Red);

                var orange = Color.FromArgb(255, 75, 0);
                var green = Color.FromArgb(0, 70, 0);
                
                //queue.Queue(new Feedback("Dim from red 1s"));
                //queue.QueueDim(1000, Color.Red);
                //queue.QueueRepeat(1);

                //queue.Queue(new Feedback("Wait 2000"));
                //queue.QueueWait(2000);

                queue.Queue(new Feedback("Pulse red 2s"));
                queue.Pulse(2000, Color.Red);

                queue.Queue(new Feedback("Pulse green 2s"));
                queue.Pulse(2000, green);

                queue.Queue(new Feedback("Pulse blue 2s"));
                queue.Pulse(2000, Color.Blue);

                queue.Queue(new Feedback("Blue 1s. Fade in 4 s."));
                queue.Color(1000, Color.Blue);
                queue.Queue(new DimAnimation(0, 0.25));
                queue.Wait(1000);

                queue.Queue(new DimAnimation(0, 0.50));
                queue.Wait(1000);

                queue.Queue(new DimAnimation(0, 0.75));
                queue.Wait(1000);

                queue.Queue(new DimAnimation(1000, 1));
                queue.Wait(1000);



                queue.Queue(new Feedback("Chase 1 orange led for 1000 for 8 times"));
                queue.Chase(1000, orange.PadBlack(8));
                queue.Repeat(8);

                queue.Queue(new Feedback("Chase 1 default orange led for 1000 for 8 times"));
                queue.Chase(1000, Color.Orange.PadBlack(8));
                queue.Repeat(8);

                queue.Queue(new Feedback("Chase 1 Yellow led for 1000 for 8 times"));
                queue.Chase(1000, Color.Yellow.PadBlack(8));
                queue.Repeat(8);

                queue.Queue(new Feedback("Off for 2000"));
                queue.Off(2000);

                queue.Queue(new Feedback("Halve green for 2000"));
                queue.Color(2000, green.PadBlack(2));

                queue.Queue(new Feedback("Full green for 2000"));
                queue.Color(2000, green);

                queue.Queue(new Feedback("Ready!"));

                /*
                queue.QueueRepeatQueue();*/

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