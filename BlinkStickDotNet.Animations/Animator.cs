using BlinkStickDotNet.Animations.Implementations;
using BlinkStickDotNet.Animations.Processors;
using System;
using System.Drawing;
using System.Threading;

namespace BlinkStickDotNet.Animations
{
    /// <summary>
    /// Queueu that store the animation. When the queue is started a thread will perform the animation.
    /// Your current thread will not be bothered.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class Animator : AnimatorBase, IDisposable
    {
        private IColorProcessor _processor;
        private Thread _thread;

        /// <summary>
        /// Occurs when the color is changed.
        /// </summary>
        public event EventHandler<ChangeColorEventArgs> ChangeColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="Animator" /> class.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="nrOfLeds">The nr of leds.</param>
        public Animator(BlinkStick stick = null, uint? nrOfLeds = null) : base(null)
        {
            if (stick != null)
            {
                Connect(stick, nrOfLeds);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get { return _thread != null; }
        }

        /// <summary>
        /// Queues the specified animation.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <returns>
        /// Queue for chaining.
        /// </returns>
        public override IAnimationQueue Queue(IAnimation animation)
        {
            var sequence = animation as SequentialAnimation;
            if (sequence != null)
            {
                foreach(var sub in sequence.GetAnimations())
                {
                    Queue(sub);
                }

                return this;
            }


            return base.Queue(animation);
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="nrOfLeds">The nr of leds.</param>
        /// <exception cref="System.Exception">No animations.</exception>
        public void Start()
        {
            if (Animations.Count == 0)
            {
                throw new Exception("No animations.");
            }

            if (_processor == null)
            {
                throw new Exception("No BlinkStick connected.");
            }

            if (_thread == null)
            {

                _thread = new Thread(() =>
                {
                    for (int i = 0; i < Animations.Count && IsRunning; i++)
                    {
                        var animation = Animations[i];

                        //check loop
                        if (animation is LoopAnimation)
                        {
                            i = -1;
                            continue;
                        }

                        animation.Start(_processor);
                    }

                    _thread = null;
                });

                _thread.Start();
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="turnOff">if set to <c>true</c> if the stick should be turned off.</param>
        public void Stop(bool turnOff = false)
        {
            try
            {
                _thread?.Abort();
                _thread = null;
            }
            catch { }

            if (turnOff)
            {
                this._processor.ProcessColors(Color.Black);
            }
        }

        /// <summary>
        /// Connects the specified stick.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="nrOfLeds">The nr of leds.</param>
        public void Connect(BlinkStick stick, uint? nrOfLeds = null)
        {
            if (stick == null)
            {
                throw new ArgumentNullException(nameof(stick));
            }

            stick.OpenDevice();

            var leds = nrOfLeds.GetValueOrDefault((uint)stick.GetLedCount());
            _processor = new ColorProcessor(stick, leds);
            _processor.ChangeColor += (sender, args) =>
            {
                this.ChangeColor?.Invoke(this, args);
            };
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            Animations.Clear();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }
    }

    [Obsolete("Please use BlinkStickDotNet.Animations.Animator")]
    public class AnimationQueue : Animator
    {
    }
}