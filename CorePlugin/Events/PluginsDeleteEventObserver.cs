
namespace CorePlugin.Events
{
    public class PluginsDeleteMessage
    {
        public int Id { get; set; }
    }
    public class PluginsDeleteEventObserver : ObserverBase<PluginsDeleteEventObserver, PluginsDeleteMessage, ushort>
    {
    }
}
