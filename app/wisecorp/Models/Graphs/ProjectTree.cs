using wisecorp.Models.DBModels;
using System.Windows.Controls;
using wisecorp.Context;
using System.Windows;
using System.Windows.Media;

namespace wisecorp.Models.Graphs;

// Adapter class to make Project work with the tree visualization
public class ProjectTreeNode : ITreeNode<Project>
{
    public Project Data { get; }
    public ITreeNode<Project>? Parent { get; set; }
    public List<ITreeNode<Project>> Children { get; } = new();

    public ProjectTreeNode(Project project)
    {
        Data = project;
    }
}

public class ProjectTree
{
    private TreeGraphView<Project> TreeGraph;

    public TreeGraphSettings<Project> Settings { get; set; } = new TreeGraphSettings<Project>
        {
            GetNodeLabel = project => project.Name,
            OnNodeClicked = project => MessageBox.Show(project.Name),
            WidthPerNode = 100,
            HeightPerNode = 40,
            HorizontalSpacing = 20,
            VerticalSpacing = 50,
            ExtraWidth = 1000,
            ExtraHeight = 1000,
        };
        

    public ProjectTree()
    {
        Settings.CreateNodeButton = (data) =>
        {
            Button nodeButton = new()
            {
                Content = Settings.GetNodeLabel(data),
                Width = Settings.WidthPerNode,
                Height = Settings.HeightPerNode,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                DataContext = data,
                Opacity = data.IsActive ? 1 : 0.5
            };
            nodeButton.SetResourceReference(Control.BackgroundProperty, "MaterialDesign.Brush.Primary.Light");
            nodeButton.SetResourceReference(Control.ForegroundProperty, "MaterialDesign.Brush.Primary.Light.Foreground");
            return nodeButton;
        };
    }

    /// <summary>
    /// Initialise un graphe en arbre vide
    /// </summary>
    public void InitializeEmptyTreeGraph()
    {
        TreeGraph = new TreeGraphView<Project>(Settings);
    }

    /// <summary>
    /// Ajoute un projet racine au graphe
    /// </summary>
    /// <param name="project">Le projet à ajouter comme racine</param>
    public void AddRoot(Project project)
    {
        var root = new ProjectTreeNode(project);
        foreach (Project p in project.ObservableEnabledSubProjects ?? Enumerable.Empty<Project>())
        {
            AddChild(root, p);
        }
        TreeGraph.AddRoot(root);
    }

    /// <summary>
    /// Ajoute un projet enfant à un nœud parent
    /// </summary>
    /// <param name="parent">Le nœud parent</param>
    /// <param name="project">Le projet à ajouter comme enfant</param>
    private void AddChild(ProjectTreeNode parent, Project project)
    {
        var child = new ProjectTreeNode(project) { Parent = parent };
        parent.Children.Add(child);
        foreach (Project p in project.ObservableEnabledSubProjects ?? Enumerable.Empty<Project>())
        {
            AddChild(child, p);
        }
    }

    /// <summary>
    /// Dessine le graphe sur le canvas spécifié
    /// </summary>
    /// <param name="graphCanvas">Le canvas sur lequel dessiner le graphe</param>
    public void DrawGraph(Canvas graphCanvas)
    {
        TreeGraph.CalculateDefaultPositions();
        TreeGraph.DrawGraph(graphCanvas);
    }
}