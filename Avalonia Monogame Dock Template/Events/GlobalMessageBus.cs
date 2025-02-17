using ReactiveUI;

namespace Avalonia_Monogame_Dock_Template.Events
{
    public static class GlobalMessageBus
    {
        public static MessageBus Instance { get; } = new MessageBus();
    }
}
