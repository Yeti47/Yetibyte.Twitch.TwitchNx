using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Services.Dialog
{
    public interface IDialogService
    {

        object? MainContext { get; set; }

        void ShowErrorDialog(string title, string message, Exception? exception = null);
    }
}
