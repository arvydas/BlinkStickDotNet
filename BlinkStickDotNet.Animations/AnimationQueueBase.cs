using System;
using System.Collections.Generic;
using System.Linq;

namespace BlinkStickDotNet.Animations
{
    /// <summary>
    /// Base class for building animation queues.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Animations.IAnimationQueue" />
    public abstract class AnimationQueueBase : IAnimationQueue
    {
        protected readonly IAnimationQueue Owner;
        protected List<IAnimation> Animations { get; private set; } = new List<IAnimation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationQueueBase"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public AnimationQueueBase(IAnimationQueue owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// Begins a parallel chain.
        /// </summary>
        /// <returns>
        /// The parallel queue for chaining.
        /// </returns>
        public IAnimationQueue BeginParallel()
        {
            var animation = new ParallelAnimation(this);
            Queue(animation);
            return animation;
        }

        /// <summary>
        /// Begins a sequencial chain.
        /// </summary>
        /// <returns>
        /// The sequencial queue for chaining.
        /// </returns>
        public IAnimationQueue BeginSequencial()
        {
            var animation = new SequentialAnimation(this);
            Queue(animation);
            return animation;
        }

        /// <summary>
        /// Returns the owner animation queue.
        /// </summary>
        /// <returns>
        /// The owner queue for chaining.
        /// </returns>
        public IAnimationQueue End()
        {
            return Owner;
        }

        /// <summary>
        /// Queues the specified animation.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <returns>Queue for chaining.</returns>
        public IAnimationQueue Queue(IAnimation animation)
        {
            this.Animations.Add(animation);
            return this;
        }

        /// <summary>
        /// Pops the specified nr of items.
        /// </summary>
        /// <param name="nrOfItems">The nr of items.</param>
        /// <returns>Queue for chaining.</returns>
        public IAnimationQueue Pop(uint nrOfItems = 1)
        {
            for (int i = 0; i < nrOfItems && Animations.Count > 0; i++)
            {
                Animations.RemoveAt(Animations.Count - 1);
            }

            return this;
        }

        /// <summary>
        /// Queues one or more repeats of the last animation.
        /// </summary>
        /// <param name="nrOfTimes">The nr of times.</param>
        /// <returns>Queue for chaining.</returns>
        public IAnimationQueue Repeat(uint nrOfTimes = 1)
        {
            if (Animations.FirstOrDefault() == null)
                throw new Exception("Can't repeat. No animations queued.");

            var animation = Animations.LastOrDefault();
            for (int i = 0; i < nrOfTimes; i++)
            {
                Queue(animation.Clone());
            }

            return this;
        }

        /// <summary>
        /// Queues a repeat of the current queue.
        /// </summary>
        /// <param name="nrOfTimes">The nr of times.</param>
        /// <returns>
        /// Queue for chaining.
        /// </returns>
        public IAnimationQueue RepeatAll(uint nrOfTimes = 1)
        {
            if (Animations.FirstOrDefault() == null)
                throw new Exception("Can't repeat. No animations queued.");

            var list = Animations.ToList();
            for (int i = 0; i < nrOfTimes; i++)
            {
                list.ForEach(a => Queue(a.Clone()));
            }

            return this;
        }

        /// <summary>
        /// Loops the queue.
        /// </summary>
        public void Loop()
        {
            if (Animations.FirstOrDefault() == null)
                throw new Exception("Can't loop. No animations queued.");

            Queue(new LoopAnimation());
        }
    }
}