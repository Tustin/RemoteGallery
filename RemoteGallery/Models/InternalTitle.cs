using Prism.Mvvm;
using RemoteGallery.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteGallery.Models
{
    public class InternalTitle : BindableBase
    {
        public string? _displayName;
        public string? DisplayName
        {
            get => _displayName ?? TitleId;
            private set => SetProperty(ref _displayName, value);
        }

        public string TitleId { get; private set; }

        private Title? _title;

        public Title? Title
        {
            get => _title;
            set => SetProperty(ref _title, value, OnTitleUpdated);
        }

        private void OnTitleUpdated() => DisplayName = Title?.Names[0].NameName;

        public InternalTitle(string titleId, ITmdbResolverService tmdbResolverService)
        {
            TitleId = titleId;

            // Fire off task to resolve the title id -> display name
            // If failed, we'll just keep using title id.
            Task.Run(async () => Title = await tmdbResolverService.GetTitle(titleId));
        }
    }
}
