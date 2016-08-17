using BlinkStickDotNet.Animations;
using BlinkStickDotNet.Usb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Threading;

namespace BlinkStickDotNet.IntegrationTests
{
    [TestClass]
    public class BlinkStickAnimationQueueIntegrationTests
    {
        [TestMethod]
        public void AnimationQueue_Morph()
        {
            var stick = BlinkStickIntegrationTests.EnsureBlinkStick();
            stick.IgnoreDisconnectErrors = true;

            var finished = new ManualResetEvent(false);

            var q = new AnimationQueue();
            q.Color(1, Color.Red);
            q.Morph(2000, Color.Blue);
            q.Morph(2000, Color.Green);
            q.Morph(2000, Color.Red);
            q.Queue(new ActionAnimation(() =>
            {
                finished.Set();
            }));

            q.Connect(stick);
            q.Start();

            finished.WaitOne();
            stick.TurnOff();
        }

        [TestMethod]
        public void AnimationQueue_Pulse()
        {
            var stick = BlinkStickIntegrationTests.EnsureBlinkStick();
            stick.IgnoreDisconnectErrors = true;

            var finished = new ManualResetEvent(false);

            var q = new AnimationQueue();
            q.Pulse(2000, Color.Blue);
            q.Pulse(2000, Color.Green);
            q.Pulse(2000, Color.Red);
            q.Queue(new ActionAnimation(() =>
            {
                finished.Set();
            }));

            q.Connect(stick);
            q.Start();

            finished.WaitOne();
            stick.TurnOff();
        }

        [TestMethod]
        public void AnimationQueue_PulseInverted()
        {
            var stick = BlinkStickIntegrationTests.EnsureBlinkStick();
            stick.IgnoreDisconnectErrors = true;

            var finished = new ManualResetEvent(false);

            var q = new AnimationQueue();
            q.PulseInverted(2000, Color.Blue);
            q.PulseInverted(2000, Color.Green);
            q.PulseInverted(2000, Color.Red);
            q.Queue(new ActionAnimation(() =>
            {
                finished.Set();
            }));

            q.Connect(stick);
            q.Start();

            finished.WaitOne();
            stick.TurnOff();
        }

        [TestMethod]
        public void AnimationQueue_Dim()
        {
            var stick = BlinkStickIntegrationTests.EnsureBlinkStick();
            stick.IgnoreDisconnectErrors = true;

            var finished = new ManualResetEvent(false);

            var q = new AnimationQueue();
            q.Color(1, Color.Red);
            q.Dim(2000);
            q.Color(1, Color.Green);
            q.Dim(2000);
            q.Color(1, Color.Blue);
            q.Dim(2000);
            q.Queue(new ActionAnimation(() =>
            {
                finished.Set();
            }));

            q.Connect(stick);
            q.Start();

            finished.WaitOne();
            stick.TurnOff();
        }


        [TestMethod]
        public void AnimationQueue_Chase()
        {
            var stick = BlinkStickIntegrationTests.EnsureBlinkStick();
            stick.IgnoreDisconnectErrors = true;

            var finished = new ManualResetEvent(false);

            var q = new AnimationQueue();
            q.Chase(1000, Color.Blue.PadBlack(8));
            q.Repeat(4);
            q.Queue(new ActionAnimation(() =>
            {
                finished.Set();
            }));

            q.Connect(stick, 8);
            q.Start();

            finished.WaitOne();
            stick.TurnOff();
        }

        [TestMethod]
        public void AnimationQueue_ChaseDimmer()
        {
            var stick = BlinkStickIntegrationTests.EnsureBlinkStick();
            stick.IgnoreDisconnectErrors = true;

            var finished = new ManualResetEvent(false);

            var q = new AnimationQueue();
            q.ChaseDimmer(5000, 5, Color.Blue.PadBlack(8));
            q.Repeat();
            q.Queue(new ActionAnimation(() =>
            {
                finished.Set();
            }));

            q.Connect(stick, 8);
            q.Start();

            finished.WaitOne();
            stick.TurnOff();
        }
    }

    class ActionAnimation : IAnimation
    {
        private Action _action;

        public ActionAnimation(Action action)
        {
            _action = action;
        }

        public IAnimation Clone()
        {
            return new ActionAnimation(_action);
        }

        public void Start(IBlinkStickColorProcessor processor)
        {
            _action();
        }
    }
}