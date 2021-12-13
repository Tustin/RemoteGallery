using FluentFTP;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteGallery.Services
{
    public interface IFtpHandler
    {
        public FtpClient FtpClient { get; }
        public bool IsActive();

    }

    public class FtpHandler : IFtpHandler
    {
        private IEventAggregator _eventAggregator;
        public FtpClient FtpClient { get; private set; }
        private CancellationTokenSource _ftpConnectCancellationToken;

        public FtpHandler(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            FtpClient = new FtpClient();
        }

        public bool IsActive() => FtpClient.IsConnected;

    }
}
