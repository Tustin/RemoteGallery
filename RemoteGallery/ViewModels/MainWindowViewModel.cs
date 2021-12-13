using FluentFTP;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using RemoteGallery.Configuration;
using RemoteGallery.Events;
using RemoteGallery.Models;
using RemoteGallery.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace RemoteGallery.ViewModels;

class MainWindowViewModel : BindableBase
{
    private IEventAggregator _eventAggregator;
    private ITmdbResolverService _tmdbResolverService;
    private IFtpHandler _ftpHandler;
    private CancellationTokenSource _ftpConnectCancellationToken;

    public ICommand ConnectToConsoleCommand { get; }

    public ObservableCollection<InternalTitle> GalleryTitles { get; private set; }

    public ICollectionView GalleryTitlesView { get; set; }

    private string _consoleIp = string.Empty;
    public string ConsoleIp
    {
        get => _consoleIp;
        set => SetProperty(ref _consoleIp, value, OnConsoleIpChanged);
    }

    private void OnConsoleIpChanged()
    {
        // 
    }

    private string _consolePort = string.Empty;
    public string ConsolePort
    {
        get => _consolePort;
        set => SetProperty(ref _consolePort, value, OnConsolePortChanged);
    }

    private void OnConsolePortChanged()
    {
        // 
    }

    private InternalTitle? _selectedTitle;

    public InternalTitle? SelectedTitle
    {
        get => _selectedTitle;
        set => SetProperty(ref _selectedTitle, value, OnSelectedTitleChanged);
    }

    private void OnSelectedTitleChanged()
    {
        _eventAggregator.GetEvent<GameChangedEvent>().Publish(SelectedTitle);
    }

    public MainWindowViewModel(IEventAggregator eventAggregator, ITmdbResolverService tmdbResolverService, IFtpHandler ftpHandler)
    {
        _eventAggregator = eventAggregator;
        _tmdbResolverService = tmdbResolverService;
        _ftpHandler = ftpHandler;

        ConnectToConsoleCommand = new DelegateCommand(ConnectToConsoleAsync, CanConnectToConsole);

#if DEBUG
        ConsoleIp = "192.168.0.116";
        ConsolePort = "2121";
#endif
        GalleryTitles = new ObservableCollection<InternalTitle>();

        GalleryTitlesView = new CollectionViewSource
        {
            Source = GalleryTitles
        }.View;

        _ftpConnectCancellationToken = new CancellationTokenSource();
    }

    private bool CanConnectToConsole()
    {
        return ! _ftpHandler.FtpClient.IsAuthenticated;
    }

    public async void ConnectToConsoleAsync()
    {
        _ftpHandler.FtpClient.Host = ConsoleIp;
        _ftpHandler.FtpClient.Port = int.Parse(ConsolePort);
        _ftpHandler.FtpClient.Credentials = new NetworkCredential("anonymous", "anonymous");

        await _ftpHandler.FtpClient.ConnectAsync(_ftpConnectCancellationToken.Token);

        foreach (FtpListItem item in await _ftpHandler.FtpClient.GetListingAsync(AppConfiguration.FullThumbnailPhotoPath))
        {
            GalleryTitles.Add(new InternalTitle(item.Name, _tmdbResolverService));
        }

    }
}
