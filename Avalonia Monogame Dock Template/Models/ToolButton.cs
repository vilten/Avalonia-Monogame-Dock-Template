using System;
using System.Linq;
using System.Reactive.Linq;
using Avalonia_Monogame_Dock_Template.Events;
using IconPacks.Avalonia.PhosphorIcons;

namespace Avalonia_Monogame_Dock_Template.Models
{
    public class ToolButton
    {
        public string Name { get; set; } = "New Tool Button";
        public PackIconPhosphorIconsKind Icon { get; set; } = PackIconPhosphorIconsKind.Selection;
        public string MessageType { get; set; } = "toolButton.newToolButton";
        private IDisposable Subscription { get; }

        public ToolButton(Action<object> onMessageReceived)
        {
            if (String.IsNullOrEmpty(MessageType) || !MessageType.StartsWith("toolButton."))
                throw new Exception("Wrong message type, should start toolButton");
            Subscription = GlobalMessageBus.Instance
                .Listen<object>() // Počúva všetky správy
                .Where(msg => msg == MessageType) // Filtrovanie podľa názvu správy
                .Subscribe(msg =>
                {
                    onMessageReceived(msg);
                });
        }

        public void Dispose()
        {
            Subscription?.Dispose(); // Odhlásenie zo správ
        }
    }
}
