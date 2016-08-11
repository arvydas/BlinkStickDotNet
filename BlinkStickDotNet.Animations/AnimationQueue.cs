using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace BlinkStickDotNet.Animations
{
    /// <summary>
    /// Queueu that store the animation. When the queue is started a thread will perform the animation.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class AnimationQueue : IAnimationQueue, IDisposable
    {
        private IBlinkStickColorProcessor _processor;
        private Thread _thread;
        public List<IAnimation> _animations = new List<IAnimation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationQueue"/> class.
        /// </summary>
        /// <param name="loop">if set to <c>true</c> the queue will loop the animation at the end.</param>
        public AnimationQueue(IBlinkStickColorProcessor processor, bool loop = false)
        {
            IsLooping = loop;

            _processor = processor;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is looping.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is looping; otherwise, <c>false</c>.
        /// </value>
        public bool IsLooping { get; private set; }

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
        public void Queue(IAnimation animation)
        {
            _animations.Add(animation);
        }

        /// <summary>
        /// Queues the specified queue.
        /// </summary>
        /// <param name="queue">The queue.</param>
        public void Queue(AnimationQueue queue)
        {
            queue.
                _animations.
                ToList().
                ForEach(a => Queue(a.Clone()));
        }

        /// <summary>
        /// Queues a repeat of the last animation.
        /// </summary>
        /// <param name="nrOfTimes">The nr of times.</param>
        public void Repeat(int nrOfTimes = 1)
        {
            if (nrOfTimes > 0 && _animations.Count > 0)
            {
                for (int i = 0; i < nrOfTimes; i++)
                {
                    var animation = _animations[_animations.Count - 1];
                    Queue(animation.Clone());
                }
            }
        }

        /// <summary>
        /// Queues a repeat of the current queue.
        /// </summary>
        /// <param name="nrOfTimes">The nr of times.</param>
        public void RepeatQueue(int nrOfTimes = 1)
        {
            if (nrOfTimes > 0 && _animations.Count > 0)
            {
                var list = _animations.ToList();

                for (int i = 0; i < nrOfTimes; i++)
                {
                    list.ForEach(a => Queue(a.Clone()));
                }
            }
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <exception cref="System.Exception">No animations.</exception>
        public void Start()
        {
            if (_thread == null)
            {
                if (_animations.Count == 0)
                {
                    throw new Exception("No animations.");
                }

                _thread = new Thread(() =>
                {
                    for (int i = 0; i < _animations.Count && IsRunning; i++)
                    {
                        _animations[i].Start(_processor);

                        if (i + 1 == _animations.Count && IsLooping)
                        {
                            i = -1;
                        }
                    }

                    _thread = null;
                });

                _thread.Start();
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            try
            {
                if (_thread != null)
                {
                    _thread.Abort();
                }
            }
            catch
            {

            }

            _thread = null;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="turnOff">if set to <c>true</c> if the stick should be turned off.</param>
        public void Stop(bool turnOff = false)
        {
            Stop();

            if (turnOff)
            {
                this._processor.ProcessColors(Color.Black);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}