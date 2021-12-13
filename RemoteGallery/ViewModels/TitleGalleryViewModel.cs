using FluentFTP;
using Prism.Events;
using Prism.Mvvm;
using RemoteGallery.Configuration;
using RemoteGallery.Events;
using RemoteGallery.Models;
using RemoteGallery.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RemoteGallery.ViewModels
{
    internal class TitleGalleryViewModel : BindableBase
    {
        private IFtpHandler _ftpHandler;
        private IEventAggregator _eventAggregator;

        public ObservableCollection<BitmapImage> TitleImages { get; private set; }

        public TitleGalleryViewModel(IEventAggregator eventAggregator, IFtpHandler ftpHandler)
        {
            _eventAggregator = eventAggregator;
            _ftpHandler = ftpHandler;

            TitleImages = new ObservableCollection<BitmapImage>();

            _eventAggregator.GetEvent<GameChangedEvent>().Subscribe(OnGameChanged);
        }

        private async void OnGameChanged(InternalTitle? title)
        {
            if (title == null)
            {
                Log.Debug("title is null");
                return;
            }

            TitleImages.Clear();

            Log.Debug($"Title changed {title.TitleId}");

            if (_ftpHandler.IsActive())
            {
                // Because GoldHEN wants you to limit simultaneous connections to 1, we will just have to download each thumbnail 1-by-1.
                // However, maybe test it out? It's slow atm.
                foreach (var item in await _ftpHandler.FtpClient.GetListingAsync($"{AppConfiguration.FullThumbnailPhotoPath}/{title.TitleId}", FtpListOption.Recursive))
                {
                    if (item.Type == FtpFileSystemObjectType.File)
                    {
                        if (Path.GetExtension(item.FullName) != ".jpg")
                        {
                            Log.Debug($"{item.FullName} - not a jpeg image");
                            continue;
                        }

                        Log.Debug($"Found {item.FullName}");

                        var localTitlePath = Path.Combine(AppConfiguration.LocalThumbnailsDirectory, title.TitleId);
                        var localThumbnailPath = Path.Combine(localTitlePath, item.Name);
                        // We could use a memory stream here, but I like being able to cache the thumbnails for later use if necessary.
                        if (!File.Exists(localThumbnailPath))
                        {
                            var status = await _ftpHandler.FtpClient.DownloadFileAsync(localThumbnailPath, item.FullName, FtpLocalExists.Skip, FtpVerify.None);
                            Log.Debug($"\t{status}");
                        }

                        TitleImages.Add(new BitmapImage(new Uri(localThumbnailPath)));
                    }
                }
            }
        }
    }
}
