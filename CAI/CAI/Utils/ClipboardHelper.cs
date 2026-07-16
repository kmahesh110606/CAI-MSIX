using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace CAI.Utils
{
    /// <summary>
    /// Clipboard integration helper for reading and writing text.
    /// </summary>
    public static class ClipboardHelper
    {
        /// <summary>Copy text to the clipboard.</summary>
        public static void CopyToClipboard(string text)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }

        /// <summary>Read text from the clipboard.</summary>
        public static async Task<string?> GetFromClipboardAsync()
        {
            var content = Clipboard.GetContent();
            if (content.Contains(StandardDataFormats.Text))
            {
                return await content.GetTextAsync();
            }
            return null;
        }
    }
}
