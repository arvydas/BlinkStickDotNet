namespace BlinkStickDotNet.Animations.Processors
{
    /// <summary>
    /// Indicates the objects implements a Led processor. Each let can be controlled seperately.
    /// The <c>Process()</c> will execute the led settings. Other threads may manipulate the same
    /// object.
    /// </summary>
    public interface ILedProcessor
    {
        /// <summary>
        /// Gets the synchronize root. Locks control of the processor.
        /// </summary>
        /// <value>
        /// The synchronize root.
        /// </value>
        object SyncRoot { get; }

        /// <summary>
        /// Gets the leds.
        /// </summary>
        /// <value>
        /// The leds.
        /// </value>
        Led[] Leds { get; }

        /// <summary>
        /// Processes this instance.
        /// </summary>
        void Process();
    }
}
