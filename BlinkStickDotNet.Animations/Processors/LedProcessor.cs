using System.Linq;

namespace BlinkStickDotNet.Animations.Processors
{
    public class LedProcessor : ILedProcessor
    {
        private readonly object _syncRoot = new object();

        IColorProcessor _stick;

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
