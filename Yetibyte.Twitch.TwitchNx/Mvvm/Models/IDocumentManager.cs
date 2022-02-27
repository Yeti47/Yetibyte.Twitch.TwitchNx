using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.Models
{
    public interface IDocumentManager
    {
        IEnumerable<DocumentViewModel> Documents { get; }
        bool HasActiveDocument { get; }

        event EventHandler<DocumentEventArgs>? DocumentOpened;
        event EventHandler<DocumentEventArgs>? DocumentClosed;

        bool OpenDocument(DocumentViewModel viewModel);
        bool CloseDocument(Type documentType, string documentName);

        bool IsDocumentOpen(DocumentViewModel viewModel);
        bool IsDocumentOpen(Type documentType, string documentName);

        bool TrySelect(Type documentType, string documentName);

    }
}
