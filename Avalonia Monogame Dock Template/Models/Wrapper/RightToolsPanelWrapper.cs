using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia_Monogame_Dock_Template.Views;

namespace Avalonia_Monogame_Dock_Template.Models.Wrapper
{
    public class RightToolsPanelWrapper(RightToolsPanel rightToolsPanel)
    {
        internal void RegisterButtonInGroup(string groupName, ToolButton toolButton)
        {
            rightToolsPanel.RegisterButtonInGroup(groupName, toolButton);
        }

        internal void RegisterGroup(string groupName)
        {
            rightToolsPanel.RegisterGroup(groupName);
        }
    }
}
