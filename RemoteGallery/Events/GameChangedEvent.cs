using Prism.Events;
using RemoteGallery.Models;

namespace RemoteGallery.Events
{
    internal class GameChangedEvent : PubSubEvent<InternalTitle?>
    {
    }
}
