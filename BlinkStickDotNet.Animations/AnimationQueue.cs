using BlinkStickDotNet.Animations.Implementations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace BlinkStickDotNet.Animations
{
    /// <summary>
    /// Queueu that store the animation. When the queue is started a thread will perform the animation.
    /// Your current thread will not be bothered.
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
        /// <param name="stick">The stick.</param>
        /// <param name="nrOfLeds">The nr of leds.</param>
        public AnimationQueue(BlinkStick stick = null, uint nrOfLeds = 1)
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
                        var animation = _animations[i];

                        //check loop
                        if(animation is LoopAnimation)
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