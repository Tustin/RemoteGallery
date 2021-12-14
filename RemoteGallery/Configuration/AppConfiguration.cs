using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteGallery.Configuration
{
    public static class AppConfiguration
    {
        public static string AppDataDirectory => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Remote Gallery"
            );
        public static string LogsDirectory => Path.Combine(AppDataDirectory, "logs");
        public static string TmpDirectory => Path.Combine(AppDataDirectory, "tmp");
        public static string ResolvedTitleIdStorage => Path.Combine(TmpDirectory, "titles.json");

        public static string LocalThumbnailsDirectory => Path.Combine(TmpDirectory, "thumbnails");

        private static string GalleryPath => "/user/av_contents/";
        private static string ThumbnailsDirectory => "thumbnails/";
        private static string VideosDirectory => "video/";
        private static string PhotosDirectory => "photo/";
        private static string ModifiedContentDirectory => "_ContExpt";
        private static string ShellUiTitleId => "NPXS20001";

        public static string FullPhotoPath => $"{GalleryPath}{PhotosDirectory}{ShellUiTitleId}";
        public static string FullVideoPath => $"{GalleryPath}{VideosDirectory}{ShellUiTitleId}";
        public static string FullThumbnailPhotoPath => $"{GalleryPath}{ThumbnailsDirectory}{PhotosDirectory}{ShellUiTitleId}";
        public static string FullThumbnailVideoPath => $"{GalleryPath}{ThumbnailsDirectory}{VideosDirectory}{ShellUiTitleId}";
    }
}
