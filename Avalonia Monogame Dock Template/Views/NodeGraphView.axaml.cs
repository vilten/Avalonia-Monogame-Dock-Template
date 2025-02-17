using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Avalonia_Monogame_Dock_Template.Views;
public partial class NodeGraphView : UserControl
{
    public string _projectPath = "../../../../Projects/test/";
    public string _projectFilename = "nodegraph.yaml";

    public NodeGraphView()
    {
        InitializeComponent();
        this.Content = new Canvas();
        var graphManager = new NodeGraphManager();
        var graph = graphManager.LoadGraph(_projectPath + _projectFilename);
        this.RenderGraph(graph);

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void RenderGraph(NodeGraph graph)
    {
        var canvas = this.Content as Canvas;
        canvas?.Children.Clear();

        foreach (var node in graph.Nodes)
        {
            var rect = new Border
            {
                Width = 100,
                Height = 50,
                Background = Brushes.LightGray,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(2),
                Child = new TextBlock { Text = node.Name, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center }
            };
            Canvas.SetLeft(rect, node.Id * 120);
            Canvas.SetTop(rect, 50);
            canvas?.Children.Add(rect);
        }
    }
}
public class Node
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<int> Connections { get; set; } = new();
}

public class NodeGraph
{
    public List<Node> Nodes { get; set; } = new();
}

public class NodeGraphManager
{
    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;

    public NodeGraphManager()
    {
        _serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
    }

    public void SaveGraph(NodeGraph graph, string filePath)
    {
        var yaml = _serializer.Serialize(graph);
        File.WriteAllText(filePath, yaml);
    }

    public NodeGraph LoadGraph(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException("File not found.");

        var yaml = File.ReadAllText(filePath);
        return _deserializer.Deserialize<NodeGraph>(yaml);
    }
}
