using BlinkStickDotNet;
using BlinkStickDotNet.Animations;
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

                var orange = Color.FromArgb(255, 75, 0);
                var green = Color.FromArgb(0, 70, 0);
                
                //queue.Queue(new Feedback("Dim from red 1s"));
                //queue.QueueDim(1000, Color.Red);
                //queue.QueueRepeat(1);

                //queue.Queue(new Feedback("Wait 2000"));
                //queue.QueueWait(2000);

                queue.Queue(new Feedback("Pulse green 1s"));
                queue.Pulse(2000, green);
                queue.Repeat(5);

                /*
                queue.QueueRepeatQueue();

                queue.Queue(new Feedback("Chase 1 orange led for 1000 for 8 times"));
                queue.QueueChase(1000, orange.PadBlack(8));
                queue.QueueRepeat(8);

                queue.Queue(new Feedback("Chase 1 default orange led for 1000 for 8 times"));
                queue.QueueChase(1000, Color.Orange.PadBlack(8));
                queue.QueueRepeat(8);

                queue.Queue(new Feedback("Chase 1 Yellow led for 1000 for 8 times"));
                queue.QueueChase(1000, Color.Yellow.PadBlack(8));
                queue.QueueRepeat(8);

                queue.Queue(new Feedback("Off for 2000"));
                queue.QueueOff(2000);

                queue.Queue(new Feedback("Halve green for 2000"));
                queue.QueueColor(2000, green.PadBlack(2));

                queue.Queue(new Feedback("Full green for 2000"));
                queue.QueueColor(2000, green);

                queue.Queue(new Feedback("Ready!"));
                */

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