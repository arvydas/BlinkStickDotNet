namespace BlinkStickDotNet.Animations
{
    public static class BlinkStickExtensions
    {
        public static IAnimationQueue CreateAnimationQueue(this BlinkStick device, bool loop = false)
        {
            var ledCount = 1;

            try
            {
                ledCount = device.GetLedCount();
            }
            catch
            {
                switch (device.BlinkStickDevice)
                {
                    case BlinkStickDeviceEnum.BlinkStickSquare:
                        ledCount = 8;
                        break;
                    default:
                        break;
                }
            }

            var processor = new BlinkStickColorProcessor(device, ledCount);

            return new AnimationQueue(processor, loop);
        }
    }
}
