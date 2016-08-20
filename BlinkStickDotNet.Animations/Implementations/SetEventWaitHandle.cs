using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BlinkStickDotNet.Animations.Processors;

namespace BlinkStickDotNet.Animations.Implementations
{
    public class SetEventWaitHandle : IAnimation
    {
        EventWaitHandle _handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetEventWaitHandle"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SetEventWaitHandle(EventWaitHandle handle)
        {
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));

            _handle = handle;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public IAnimation Clone()
        {
            return new SetEventWaitHandle(_handle);
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public void Start(ILedProcessor processor)
        {
            _handle.Set();
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public void Start(IColorProcessor processor)
        {
            _handle.Set();
        }
    }
}
