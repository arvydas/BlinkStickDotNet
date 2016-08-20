using System.Linq;

namespace BlinkStickDotNet.Animations.Processors
{
    /// <summary>
    /// Implements a led processor for a single stick.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Animations.Processors.ILedProcessor" />
    public class LedProcessor : ILedProcessor
    {
        private readonly object _syncRoot = new object();
        private IColorProcessor _stick;

        /// <summary>
        /// Initializes a new instance of the <see cref="LedProcessor"/> class.
        /// </summary>
        /// <param name="stick">The stick.</param>
        public LedProcessor(IColorProcessor stick)
        {
            _stick = stick;

            uint i = 0;

            this.Leds = stick
                .GetCurrentColors()
                .CloneArray((int)_stick.NrOfLeds)
                .Select(c => new Led(i++, c))
                .ToArray();

        }

        /// <summary>
        /// Gets the synchronize root. Locks control of the processor.
        /// </summary>
        /// <value>
        /// The synchronize root.
        /// </value>
        public object SyncRoot { get { return _syncRoot; } }

        /// <summary>
        /// Gets the leds.
        /// </summary>
        /// <value>
        /// The leds.
        /// </value>
        public Led[] Leds
        {
            get;
            private set;
        }

        /// <summary>
        /// Processes this instance.
        /// </summary>
        public void Process()
        {
            lock (_syncRoot)
            {
                var colors = this.Leds
                    .OrderBy(l => l.LedNr)
                    .Select(l => l.Color)
                    .ToArray();

                _stick.ProcessColors(colors);
            }
        }
    }
}