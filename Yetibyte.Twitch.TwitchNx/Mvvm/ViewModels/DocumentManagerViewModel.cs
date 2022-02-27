using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yetibyte.Twitch.TwitchNx.Mvvm.Models;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels.Layout;

namespace Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels
{
    public class DocumentManagerViewModel : IDocumentManager
    {
        private readonly ObservableCollection<DocumentViewModel> _documents = new ObservableCollection<DocumentViewModel>();

        public IEnumerable<DocumentViewModel> Documents => _documents;

        public bool HasActiveDocument => _documents.Any(d => d.IsActive);

        public event EventHandler<DocumentEventArgs>? DocumentOpened;
        public event EventHandler<DocumentEventArgs>? DocumentClosed;

        public bool CloseDocument(Type documentType, string documentName)
        {
            if (!IsDocumentOpen(documentType, documentName))
                return false;

            DocumentViewModel doc = _documents.First(d => d.DocumentName == documentName && d.DocumentType == documentType);

            _documents.Remove(doc);

            return true;
        }

        public bool OpenDocument(DocumentViewModel viewModel)
        {
            if (IsDocumentOpen(viewModel))
                return false;

            _documents.Add(viewModel);

            return true;
        }

        protected virtual void OnDocumentOpened(DocumentViewModel documentViewModel)
        {
            var handler = DocumentOpened;
            handler?.Invoke(this, new DocumentEventArgs(documentViewModel));   
        }

        protected virtual void OnDocumentClosed(DocumentViewModel documentViewModel)
        {
            var handler = DocumentClosed;
            handler?.Invoke(this, new DocumentEventArgs(documentViewModel));
        }

        public bool IsDocumentOpen(DocumentViewModel viewModel)
        {
            return _documents.Contains(viewModel)
                || IsDocumentOpen(viewModel.DocumentType, viewModel.DocumentName);
        }

        public bool IsDocumentOpen(Type documentType, string documentName)
        {
            return _documents.Any(d => d.DocumentName == documentName && d.DocumentType == documentType);
        }

        public bool TrySelect(Type documentType, string documentName)
        {
            if (!IsDocumentOpen(documentType, documentName))
                return false;

            foreach (var document in _documents)
            {
                document.IsSelected = false;
                document.IsActive = false;
            }

            DocumentViewModel doc = _documents.First(d => d.DocumentName == documentName && d.DocumentType == documentType);
            doc.IsSelected = true;
            doc.IsActive = true;
            

            return true;
        }
    }
}
