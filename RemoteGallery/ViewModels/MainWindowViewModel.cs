using FluentFTP;
using Prism.Commands;
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
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace RemoteGallery.ViewModels;

class MainWindowViewModel : BindableBase
{
    private readonly IEventAggregator _eventAggregator;
    private readonly ITmdbResolverService _tmdbResolverService;
    private readonly IFtpHandler _ftpHandler;
    private readonly CancellationTokenSource _ftpConnectCancellationToken;

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

    private bool _isConnecting = false;
    public bool IsConnecting
    {
        get => _isConnecting;
        set => SetProperty(ref _isConnecting, value);
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

    private string _searchQuery = string.Empty;

    public string SearchQuery
    {
        get => _searchQuery;
        set => SetProperty(ref _searchQuery, value, OnSearchQueryChanged);
    }

    private void OnSearchQueryChanged()
    {
        GalleryTitlesView.Refresh();
    }

    public MainWindowViewModel(IEventAggregator eventAggregator, ITmdbResolverService tmdbResolverService, IFtpHandler ftpHandler)
    {
        _eventAggregator = eventAggregator;
        _tmdbResolverService = tmdbResolverService;
        _ftpHandler = ftpHandler;

        ConnectToConsoleCommand = new DelegateCommand(ConnectToConsoleAsync, CanConnectToConsole).ObservesProperty(() => IsConnecting);

#if DEBUG
        ConsoleIp = "192.168.0.116";
        ConsolePort = "2121";
#endif
        GalleryTitles = new ObservableCollection<InternalTitle>();


        var cvs = new CollectionViewSource
        {
            Source = GalleryTitles,
        };

        GalleryTitlesView = cvs.View;
        GalleryTitlesView.Filter = OnFilterTitle;

        _ftpConnectCancellationToken = new CancellationTokenSource();
    }

    private bool OnFilterTitle(object obj)
    {
        if (string.IsNullOrEmpty(SearchQuery))
        {
            return true;
        }

        var title = obj as InternalTitle;

        if (title == null)
        {
            return false;
        }

        return title.DisplayName.Contains(SearchQuery, StringComparison.CurrentCultureIgnoreCase) || title.TitleId.Contains(SearchQuery, StringComparison.CurrentCultureIgnoreCase);
    }

    private bool CanConnectToConsole()
    {
        return !IsConnecting && ! _ftpHandler.IsActive();
    }

    public async void ConnectToConsoleAsync()
    {
        IsConnecting = true;

        _ftpHandler.FtpClient.Host = ConsoleIp;
        _ftpHandler.FtpClient.Port = int.Parse(ConsolePort);
        _ftpHandler.FtpClient.Credentials = new NetworkCredential("anonymous", "anonymous");

        try
        {
            await _ftpHandler.FtpClient.ConnectAsync(_ftpConnectCancellationToken.Token);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed connecting to console");
            MessageBox.Show("Failed connecting to console. Make sure your console IP and port is correct.", "Remote Gallery");
            return;
        }
        finally
        {
            IsConnecting = false;
        }

        foreach (FtpListItem item in await _ftpHandler.FtpClient.GetListingAsync(AppConfiguration.FullThumbnailPhotoPath))
        {
            GalleryTitles.Add(new InternalTitle(item.Name, _tmdbResolverService));
        }
    }
}
