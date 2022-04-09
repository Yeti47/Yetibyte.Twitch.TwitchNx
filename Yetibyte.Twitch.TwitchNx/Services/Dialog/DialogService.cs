using MahApps.Metro.Controls.Dialogs;
using System;

namespace Yetibyte.Twitch.TwitchNx.Services.Dialog
{
    public class DialogService : IDialogService
    {
        public object? MainContext { get; set; }

        public void ShowErrorDialog(string title, string message, Exception? exception = null)
        {
            string messageOut = message;

            if (exception is not null)
            {
                messageOut += $"\r\n\r\nDetails:\r\n{exception.Message}";
            }

            DialogCoordinator.Instance.ShowMessageAsync(
                MainContext, 
                title, 
                messageOut, 
                MessageDialogStyle.Affirmative, 
                new MetroDialogSettings { 
                    ColorScheme = MetroDialogColorScheme.Inverted 
                }
            );
        }
    }
}
