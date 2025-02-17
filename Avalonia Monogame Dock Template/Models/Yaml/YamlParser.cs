using System;
using System.Collections.ObjectModel;
using System.IO;
using YamlDotNet.RepresentationModel;

namespace Avalonia_Monogame_Dock_Template.Models.Yaml
{
    public class YamlParser
    {
        public static ObservableCollection<YamlNode> LoadYaml(string yamlText)
        {
            var nodes = new ObservableCollection<YamlNode>();
            var yamlStream = new YamlStream();
            using (var reader = new StringReader(yamlText))
            {
                yamlStream.Load(reader);
            }

            foreach (YamlDocument document in yamlStream.Documents)
            {
                var root = ConvertYamlNode(document.RootNode, "Root", "FileDocument");
                nodes.Add(root);
            }

            return nodes;
        }

        private static YamlNode ConvertYamlNode(YamlDotNet.RepresentationModel.YamlNode yamlNode, string nodeName, string icon)
        {
            var treeNode = new YamlNode(nodeName, icon);

            if (yamlNode is YamlMappingNode mapping)
            {
                foreach (var entry in mapping.Children)
                {
                    var key = entry.Key.ToString();
                    var newIcon = GetIconForKey(key);
                    var child = ConvertYamlNode(entry.Value, key, newIcon);
                    treeNode.Children.Add(child);
                }
            }
            else if (yamlNode is YamlSequenceNode sequence)
            {
                int index = 0;
                foreach (var item in sequence.Children)
                {
                    var child = ConvertYamlNode(item, $"Item {index++}", "VectorPoint");
                    treeNode.Children.Add(child);
                }
            }
            else
            {
                treeNode.Name += $": {yamlNode}";
            }

            return treeNode;
        }

        private static string GetIconForKey(string key)
        {
            return key switch
            {
                "settings" => "Folder",
                "layers" => "Stack",
                "layerItems" => "StackSimple",
                "vertices" => "DotsSix",
                _ => "File"
            };
        }
    }
}
