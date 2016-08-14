using System.Drawing;
using System.Windows.Forms;

namespace BlinkStickDotNet.IntegrationTests
{
    /// <summary>
    /// Creates a message box that can be closes by a different thread.
    /// Uses native window forms elements.
    /// </summary>
    public class MessageBox
    {
        private string _text;
        private Form _owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBox"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public MessageBox(string text = null)
        {
            _text = text;
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            _owner?.Close();
            _owner?.Dispose();
            _owner = null;
        }

        /// <summary>
        /// Shows the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void Show(string text = null)
        {
            _owner = new Form() { Size = new Size(0, 0) };

            System.Windows.Forms.MessageBox.Show(
                    _owner, 
                    text ?? _text ?? "Why am I here? I've got nothing to say...",
                    "BlinkStick Integration Test", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Question, 
                    MessageBoxDefaultButton.Button1, 
                    (MessageBoxOptions)0x40000
            );

            Close();
        }
    }
}