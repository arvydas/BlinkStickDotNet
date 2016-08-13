using System;
using System.Collections;
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
        /// Initializes a new instance of the <see cref="AnimationQueue" /> class.
        /// </summary>
        /// <param name="loop">if set to <c>true</c> the queue will loop the animation at the end.</param>
        /// <param name="stick">The stick.</param>
        /// <param name="nrOfLeds">The nr of leds.</param>
        public AnimationQueue(bool loop = false, BlinkStick stick = null, uint nrOfLeds = 1)
        {
            IsLooping = loop;

            if (stick != null)
            {
                Connect(stick, nrOfLeds);
            }
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
        /// Pops the specified nr of items.
        /// </summary>
        /// <param name="nrOfItems">The nr of items.</param>
        public void Pop(int nrOfItems = 1)
        {
            for (int i = 0; i < nrOfItems && _animations.Count > 0; i++)
            {
                _animations.RemoveAt(_animations.Count - 1);
            }
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
        /// <param name="stick">The stick.</param>
        /// <param name="nrOfLeds">The nr of leds.</param>
        /// <exception cref="System.Exception">No animations.</exception>
        public void Start()
        {
            if (_animations.Count == 0)
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
        public void Connect(BlinkStick stick, uint nrOfLeds = 1)
        {
            if (stick == null)
            {
                throw new ArgumentNullException(nameof(stick));
            }

            _processor = new BlinkStickColorProcessor(stick, nrOfLeds);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            _animations.Clear();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IAnimation> GetEnumerator()
        {
            return _animations.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _animations.GetEnumerator();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }
    }
}