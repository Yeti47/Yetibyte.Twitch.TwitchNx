using System;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.Models
{
    public class DocumentEventArgs : EventArgs
    {
        public DocumentViewModel Document { get; }

        public DocumentEventArgs(DocumentViewModel document)
        {
            Document = document;
        }
    }
}
