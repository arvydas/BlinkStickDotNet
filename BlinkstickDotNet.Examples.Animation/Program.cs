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
                var stick = new BlinkStickColorProcessor(device, 8);
                var queue = new AnimationQueue();
                var orange = Color.FromArgb(255, 75, 0);
                var green = Color.FromArgb(0, 70, 0);

                queue.Queue(new ConsoleAnimation("Dim from red: 200, 0.1"));
                queue.QueueDim(200, 0.1, Color.Red);
                queue.Queue(new ConsoleAnimation("Wait 2000"));
                queue.QueueWait(2000);
                queue.QueueRepeatQueue();

                queue.Queue(new ConsoleAnimation("Chase 1 orange led for 1000 for 8 times"));
                queue.QueueChase(1000, orange.PadBlack(8));
                queue.QueueRepeat(8);

                queue.Queue(new ConsoleAnimation("Off for 2000"));
                queue.QueueOff(2000);

                queue.Queue(new ConsoleAnimation("Halve green for 2000"));
                queue.QueueColor(2000, green.PadBlack(2));

                queue.Queue(new ConsoleAnimation("Ready!"));
                queue.Start(stick);

                try
                {
                    Console.ReadLine();
                }
                finally
                {
                    queue.Stop();
                    stick.Off();
                }
            }
        }
    }
}