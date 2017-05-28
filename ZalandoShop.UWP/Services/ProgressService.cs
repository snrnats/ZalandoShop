using System;
using System.ComponentModel;
using Microsoft.Toolkit.Uwp;
using Nito.Disposables;

namespace ZalandoShop.UWP.Services
{
    public class ProgressService : INotifyPropertyChanged
    {
        private int _backgroundOperationCount;
        private int _uiOperationCount;

        public bool UiOperation => _uiOperationCount > 0;
        public bool BackgroundOperation => _backgroundOperationCount > 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public IDisposable BeginUiOperation(bool marshallToUiThread = false)
        {
            _uiOperationCount++;
            OnPropertyChanged(nameof(UiOperation));
            return new AnonymousDisposable(async () =>
            {
                if (--_uiOperationCount == 0)
                {
                    if (marshallToUiThread)
                    {
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() => OnPropertyChanged(nameof(UiOperation)));
                    }
                    else
                    {
                        OnPropertyChanged(nameof(UiOperation));
                    }
                }
            });
        }

        public IDisposable BeginBackgroundOperation(bool marshallToUiThread = false)
        {
            _backgroundOperationCount++;
            OnPropertyChanged(nameof(BackgroundOperation));
            return new AnonymousDisposable(async () =>
            {
                if (--_backgroundOperationCount == 0)
                {
                    if (marshallToUiThread)
                    {
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() => OnPropertyChanged(nameof(BackgroundOperation)));
                    }
                    else
                    {
                        OnPropertyChanged(nameof(BackgroundOperation));
                    }
                }
            });
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
