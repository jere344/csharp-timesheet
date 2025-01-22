using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace wisecorp.Models.Graphs;

// Interface for objects that can be represented in the tree
public interface ITreeNode<T>
{
    T Data { get; }
    ITreeNode<T>? Parent { get; set; }
    List<ITreeNode<T>> Children { get; }
}

// Settings class with added edge operation callbacks
public class TreeGraphSettings<T>
{
    public int WidthPerNode { get; set; } = 100;
    public int HeightPerNode { get; set; } = 40;
    public int HorizontalSpacing { get; set; } = 20;
    public int VerticalSpacing { get; set; } = 50;
    public int ExtraWidth { get; set; } = 1000;
    public int ExtraHeight { get; set; } = 1000;
    public Func<T, string> GetNodeLabel { get; set; } = (data) => data?.ToString() ?? string.Empty;
    
    public Action<T>? OnNodeClicked { get; set; }

    // Callbacks for edge operations
    public Func<ITreeNode<T>, ITreeNode<T>, bool>? OnBeforeEdgeRemoved { get; set; } // Return false to prevent edge removal
    public Action<ITreeNode<T>, ITreeNode<T>>? OnEdgeRemoved { get; set; } // Called after an edge is removed
    public Func<ITreeNode<T>, ITreeNode<T>, bool>? OnBeforeEdgeAdded { get; set; } // Return false to prevent edge addition
    public Action<ITreeNode<T>, ITreeNode<T>>? OnEdgeAdded { get; set; } // Called after an edge is added


    public Func<T, Button> CreateNodeButton { get; set; }
    public Func<T, Grid> CreateAddEdgeButtonGrid { get; set; }
    public Func<int, int, int, int, Path> CreateEdgePath { get; set; }
    public Func<double, double, Polygon> CreateArrowHead { get; set; }
    public Func<double, double, Grid> CreateDeleteButtonGrid { get; set; }

    public TreeGraphSettings()
    {
        CreateNodeButton = (data) =>
        {
            Button nodeButton = new()
            {
                Content = GetNodeLabel(data),
                Width = WidthPerNode,
                Height = HeightPerNode,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                DataContext = data
            };
            nodeButton.SetResourceReference(Control.BackgroundProperty, "MaterialDesign.Brush.Primary.Light");
            nodeButton.SetResourceReference(Control.ForegroundProperty, "MaterialDesign.Brush.Primary.Light.Foreground");
            return nodeButton;
        };

        CreateAddEdgeButtonGrid = (data) =>
        {
            var addEdgeGrid = new Grid();
            var ellipse = new Ellipse
            {
                Width = 20,
                Height = 20,
                StrokeThickness = 1,
                Stroke = Brushes.Transparent,
                Fill = Brushes.Transparent
            };
            addEdgeGrid.Children.Add(ellipse);

            var icon = new MaterialDesignThemes.Wpf.PackIcon
            {
                Kind = MaterialDesignThemes.Wpf.PackIconKind.PlusCircleOutline,
                Width = 20,
                Height = 20,
            };
            icon.SetResourceReference(Control.ForegroundProperty, "MaterialDesignBody");
            addEdgeGrid.Children.Add(icon);

            return addEdgeGrid;
        };

        CreateEdgePath = (startX, startY, endX, endY) =>
        {
            double controlY = (startY + endY) / 2 - 20;

            Path path = new() { StrokeThickness = 2 };
            path.SetResourceReference(Shape.StrokeProperty, "MaterialDesignDivider");
            PathGeometry pathGeometry = new();
            PathFigure pathFigure = new() { StartPoint = new Point(startX, startY) };
            BezierSegment bezierSegment = new(
                new Point(startX, controlY),
                new Point(endX, controlY),
                new Point(endX, endY),
                true
            );
            pathFigure.Segments.Add(bezierSegment);
            pathGeometry.Figures.Add(pathFigure);
            path.Data = pathGeometry;

            return path;
        };

        CreateArrowHead = (endX, endY) =>
        {
            Polygon arrowHead = new()
            {
                Points = new PointCollection
                {
                    new Point(endX, endY),
                    new Point(endX - 5, endY - 10),
                    new Point(endX + 5, endY - 10)
                }
            };
            arrowHead.SetResourceReference(Shape.FillProperty, "MaterialDesignDivider");
            return arrowHead;
        };

        CreateDeleteButtonGrid = (x, y) =>
        {
            var deleteEdgeGrid = new Grid();
            var ellipse = new Ellipse
            {
                Width = 20,
                Height = 20,
                StrokeThickness = 1,
                Stroke = Brushes.Transparent,
                Fill = Brushes.Transparent
            };
            deleteEdgeGrid.Children.Add(ellipse);

            var icon = new MaterialDesignThemes.Wpf.PackIcon
            {
                Kind = MaterialDesignThemes.Wpf.PackIconKind.SelectionEllipseRemove,
                Width = 20,
                Height = 20,
            };
            icon.SetResourceReference(Control.ForegroundProperty, "MaterialDesignBody");
            deleteEdgeGrid.Children.Add(icon);

            return deleteEdgeGrid;
        };
    }
}

// Generic tree node visualization class
public class TreeNodeView<T>
{
    public ITreeNode<T> Node { get; }
    public TreeGraphView<T> GraphView { get; }
    private (Path? Arrow, Polygon? ArrowHead, Grid? DeleteButton) EdgeVisualizations { get; set; } = (null, null, null);

    public TreeNodeView(ITreeNode<T> node, TreeGraphView<T> graphView)
    {
        Node = node;
        GraphView = graphView;
    }

    /// <summary>
    /// Get the depth of the node in the tree. The root node has depth 0.
    /// </summary>
    /// <returns></returns>
    public int GetDepth()
    {
        if (Node.Parent == null)
        {
            return 0;
        }
        return 1 + GraphView.GetNodeView(Node.Parent).GetDepth();
    }

    /// <summary>
    /// Get the width of the node in the tree
    /// The width is the sum of the widths of all the children nodes plus the horizontal spacing between them
    /// </summary>
    /// <returns></returns>
    public int GetWidth()
    {
        if (Node.Children.Count == 0)
        {
            return GraphView.Settings.WidthPerNode;
        }
        return Node.Children.Sum(c => GraphView.GetNodeView(c).GetWidth()) +
               (Node.Children.Count - 1) * GraphView.Settings.HorizontalSpacing;
    }

    /// <summary>
    /// The nodes have default positions calculated based on the depth and width of the tree
    /// the positions are the upper left corner of the node
    /// </summary>
    /// <param name="nodePositions"></param>
    /// <param name="startX"></param>
    public void CalculateDefaultPositions(Dictionary<ITreeNode<T>, (int x, int y)> nodePositions, int startX)
    {
        int y = GraphView.Settings.ExtraHeight + GetDepth() *
                (GraphView.Settings.HeightPerNode + GraphView.Settings.VerticalSpacing);
        int x = GraphView.Settings.ExtraWidth + startX +
                (GetWidth() - GraphView.Settings.WidthPerNode) / 2;
        nodePositions.Add(Node, (x, y));

        int currentX = startX;
        foreach (var child in Node.Children)
        {
            GraphView.GetNodeView(child).CalculateDefaultPositions(nodePositions, currentX);
            currentX += GraphView.GetNodeView(child).GetWidth() + GraphView.Settings.HorizontalSpacing;
        }
    }

    /// <summary>
    /// Draw the node on the canvas at the specified position
    /// </summary>
    /// <param name="graphCanvas"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void DrawNode(Canvas graphCanvas, int x, int y)
    {
        var nodeButton = GraphView.Settings.CreateNodeButton(Node.Data);

        Panel.SetZIndex(nodeButton, 1);

        Canvas.SetLeft(nodeButton, x);
        Canvas.SetTop(nodeButton, y);

        graphCanvas.Children.Add(nodeButton);

        nodeButton.Click += (sender, e) => GraphView.Settings.OnNodeClicked?.Invoke(Node.Data);

        DrawNodeAddEdgeButton(graphCanvas, x + GraphView.Settings.WidthPerNode / 2,
                            y + GraphView.Settings.HeightPerNode);
    }

    /// <summary>
    /// Draw the button to add an edge from the node
    /// When the button is clicked, the user can drag the mouse to the top of another node to create an edge
    /// </summary>
    /// <param name="graphCanvas"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void DrawNodeAddEdgeButton(Canvas graphCanvas, int x, int y)
    {
        var addEdgeGrid = GraphView.Settings.CreateAddEdgeButtonGrid(Node.Data);

        Panel.SetZIndex(addEdgeGrid, 2);
        Canvas.SetLeft(addEdgeGrid, x - 10);
        Canvas.SetTop(addEdgeGrid, y - 10);

        graphCanvas.Children.Add(addEdgeGrid);

        addEdgeGrid.Opacity = 0;
        addEdgeGrid.MouseEnter += (s, e) => { addEdgeGrid.Opacity = 1; };
        addEdgeGrid.MouseLeave += (s, e) => { addEdgeGrid.Opacity = 0; };

        addEdgeGrid.MouseLeftButtonDown += (s, e) =>
        {
            e.Handled = true;

            addEdgeGrid.CaptureMouse();
            Point start = e.GetPosition(graphCanvas);
            double startX = x;
            double startY = y;

            Path path = new() { StrokeThickness = 2 };
            path.SetResourceReference(Shape.StrokeProperty, "MaterialDesignDivider");

            // Create a dragging indicator icon
            var dragIndicator = new MaterialDesignThemes.Wpf.PackIcon
            {
                Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDownBold,
                Width = 24,
                Height = 24,
                Foreground = Brushes.Red
            };
            Panel.SetZIndex(dragIndicator, 3);
            graphCanvas.Children.Add(dragIndicator);

            PathGeometry pathGeometry = new();
            PathFigure pathFigure = new() { StartPoint = new Point(startX, startY) };
            pathGeometry.Figures.Add(pathFigure);
            path.Data = pathGeometry;

            graphCanvas.Children.Add(path);

            void DragMoveHandler(object sender, System.Windows.Input.MouseEventArgs me)
            {
                if (addEdgeGrid.IsMouseCaptured)
                {
                    Point currentPosition = me.GetPosition(graphCanvas);
                    pathFigure.Segments.Clear();
                    pathFigure.Segments.Add(new BezierSegment(
                        new Point(startX, startY + 50),
                        new Point(currentPosition.X, startY + 50),
                        new Point(currentPosition.X, currentPosition.Y),
                        true
                    ));

                    // Update indicator position
                    Canvas.SetLeft(dragIndicator, currentPosition.X - 12);
                    Canvas.SetTop(dragIndicator, currentPosition.Y - 12);

                    // Check if we're over a valid target
                    // we can use nodePositions to check if the target is a valid node
                    // We could use a hit test as it would be more accurate, but this is simpler and more performant
                    // Don't move around the node manually and it will work fine
                    var element = GraphView.NodePositions.Keys.FirstOrDefault(n =>
                    {
                        var (nx, ny) = GraphView.NodePositions[n];
                        return nx < currentPosition.X && currentPosition.X < nx + GraphView.Settings.WidthPerNode &&
                               ny < currentPosition.Y && currentPosition.Y < ny + GraphView.Settings.HeightPerNode;
                    });

                    // Check if the target is valid (exists, not self, not a descendant)
                    bool isValidTarget = element != null && element != Node && !GraphView.IsDescendant(Node, element);

                    // Update indicator appearance
                    dragIndicator.Foreground = isValidTarget ? Brushes.Green : Brushes.Red;
                    dragIndicator.Kind = isValidTarget ?
                        MaterialDesignThemes.Wpf.PackIconKind.ArrowDownBoldCircle :
                        MaterialDesignThemes.Wpf.PackIconKind.ArrowDownBold;
                }
            }

            void DragReleaseHandler(object sender, System.Windows.Input.MouseEventArgs me)
            {
                addEdgeGrid.ReleaseMouseCapture();
                graphCanvas.MouseMove -= DragMoveHandler;
                graphCanvas.MouseLeftButtonUp -= DragReleaseHandler;
                graphCanvas.Children.Remove(path);
                graphCanvas.Children.Remove(dragIndicator);

                var currentPosition = me.GetPosition(graphCanvas);

                var targetElement = GraphView.NodePositions.Keys.FirstOrDefault(n =>
                {
                    var (nx, ny) = GraphView.NodePositions[n];
                    return nx < currentPosition.X && currentPosition.X < nx + GraphView.Settings.WidthPerNode &&
                            ny < currentPosition.Y && currentPosition.Y < ny + GraphView.Settings.HeightPerNode;
                });

                if (targetElement != null)
                {
                    GraphView.TryAddEdge(Node, targetElement);
                };
            }

            graphCanvas.MouseMove += DragMoveHandler;
            graphCanvas.MouseLeftButtonUp += DragReleaseHandler;
        };
    }

    /// <summary>
    /// Draw an edge from the specified start to end coordinates using a bezier curve
    /// </summary>
    /// <param name="graphCanvas"></param>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    /// <param name="endX"></param>
    /// <param name="endY"></param>
    /// <returns></returns>
    public void DrawEdge(Canvas graphCanvas, double startX, double startY, double endX, double endY)
    {
        Path path = GraphView.Settings.CreateEdgePath((int)startX, (int)startY, (int)endX, (int)endY);
        graphCanvas.Children.Add(path);

        Polygon arrowHead = GraphView.Settings.CreateArrowHead(endX, endY);
        graphCanvas.Children.Add(arrowHead);

        Grid deleteEdgeButton = DrawEdgeDeleteButton(graphCanvas, endX, endY);

        EdgeVisualizations = (path, arrowHead, deleteEdgeButton);
    }

    /// <summary>
    /// Draw a button to delete the edge
    /// </summary>
    /// <param name="graphCanvas"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="arrow"></param>
    /// <param name="arrowHead"></param>
    private Grid DrawEdgeDeleteButton(Canvas graphCanvas, double x, double y)
    {
        var deleteEdgeGrid = GraphView.Settings.CreateDeleteButtonGrid(x, y);
        Canvas.SetLeft(deleteEdgeGrid, x - 10);
        Canvas.SetTop(deleteEdgeGrid, y - 10);
        Panel.SetZIndex(deleteEdgeGrid, 2);
        graphCanvas.Children.Add(deleteEdgeGrid);

        deleteEdgeGrid.Opacity = 0;
        deleteEdgeGrid.MouseEnter += (s, e) => { deleteEdgeGrid.Opacity = 1; };
        deleteEdgeGrid.MouseLeave += (s, e) => { deleteEdgeGrid.Opacity = 0; };

        deleteEdgeGrid.MouseLeftButtonDown += (s, e) =>
        {
            GraphView.TryRemoveNodeParent(Node);
        };

        return deleteEdgeGrid;
    }

    /// <summary>
    /// Remove the edge visualization from the canvas
    /// </summary>
    public void RemoveEdge()
    {
        if (EdgeVisualizations != (null, null, null) && GraphView.GraphCanvas != null)
        {
            GraphView.GraphCanvas.Children.Remove(EdgeVisualizations.Arrow);
            GraphView.GraphCanvas.Children.Remove(EdgeVisualizations.ArrowHead);
            GraphView.GraphCanvas.Children.Remove(EdgeVisualizations.DeleteButton);
        }
    }
}


// Main tree graph visualization class
public class TreeGraphView<T>
{
    private readonly List<ITreeNode<T>> roots = new();
    private readonly Dictionary<ITreeNode<T>, TreeNodeView<T>> nodeViews = new();
    public TreeGraphSettings<T> Settings { get; }
    public Dictionary<ITreeNode<T>, (int x, int y)> NodePositions { get; } = new();
    public Canvas? GraphCanvas { get; private set; }

    public TreeGraphView(TreeGraphSettings<T> settings)
    {
        Settings = settings;
    }

    /// <summary>
    /// The tree can have multiple roots
    /// </summary>
    /// <param name="root"></param>
    public void AddRoot(ITreeNode<T> root)
    {
        roots.Add(root);
        CreateNodeViews(root);
    }

    /// <summary>
    /// Create the node views for root and all its children
    /// </summary>
    /// <param name="node"></param>
    private void CreateNodeViews(ITreeNode<T> node)
    {
        nodeViews[node] = new TreeNodeView<T>(node, this);
        foreach (var child in node.Children)
        {
            CreateNodeViews(child);
        }
    }

    public TreeNodeView<T> GetNodeView(ITreeNode<T> node) => nodeViews[node];

    /// <summary>
    /// Try to remove the parent of the child node in the tree. 
    /// return true if there is no parent after the call, false if there's still is
    /// </summary>
    /// <param name="child"></param>
    public bool TryRemoveNodeParent(ITreeNode<T> child)
    {
        if (child.Parent == null) return true;

        if (!(Settings.OnBeforeEdgeRemoved?.Invoke(child.Parent!, child) ?? true)) return false;

        // remove the edge visualization
        GetNodeView(child).RemoveEdge();

        // Add to the roots if it's not there
        if (!roots.Contains(child))
        {
            roots.Add(child);
        }

        child.Parent.Children.Remove(child);
        child.Parent = null;

        Settings.OnEdgeRemoved?.Invoke(child.Parent!, child);
        return true;
    }

    /// <summary>
    /// Try to add an edge between parent and child nodes
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="child"></param>
    /// <returns></returns>
    public bool TryAddEdge(ITreeNode<T> parent, ITreeNode<T> child)
    {
        // add edge fail if :

        // it would cause a loop
        if (IsDescendant(child, parent)) { return false; }
        // the custom function refused
        if (!(Settings.OnBeforeEdgeAdded?.Invoke(parent, child) ?? true)) { return false; }
        // the current parent can't be removed
        if (!TryRemoveNodeParent(child)) { return false; }
        // we have no canvas
        if (GraphCanvas == null) { return false; }


        // first draw the graphic part
        GetNodeView(child).DrawEdge(GraphCanvas, 
            NodePositions[parent].x + (Settings.WidthPerNode / 2), 
            NodePositions[parent].y + Settings.HeightPerNode,
            NodePositions[child].x + (Settings.WidthPerNode / 2), 
            NodePositions[child].y
        );

        // Remove from the roots if it's there
        roots.Remove(child);

        // Add the new edge
        parent.Children.Add(child);
        child.Parent = parent;

        // Notify that the edge was added
        Settings.OnEdgeAdded?.Invoke(parent, child);

        return true;
    }

    /// <summary>
    /// Check if node is a descendant of ancestor
    /// </summary>
    /// <param name="node"></param>
    /// <param name="ancestor"></param>
    /// <returns></returns>
    public bool IsDescendant(ITreeNode<T> node, ITreeNode<T> ancestor)
    {
        if (node == ancestor)
        {
            return true;
        }
        if (node.Parent == null)
        {
            return false;
        }
        return IsDescendant(node.Parent, ancestor);
    }

    /// <summary>
    /// Calculate the default positions of the nodes in the tree
    /// </summary>
    public void CalculateDefaultPositions()
    {
        NodePositions.Clear();
        int startX = 0;
        foreach (var root in roots)
        {
            GetNodeView(root).CalculateDefaultPositions(NodePositions, startX);
            startX += GetNodeView(root).GetWidth() + Settings.HorizontalSpacing;
        }
    }

    /// <summary>
    /// Draw the tree on the canvas
    /// </summary>
    public void DrawGraph(Canvas graphCanvas)
    {
        GraphCanvas = graphCanvas;
        DrawNodes();
    }

    /// <summary>
    /// Draw all the nodes in the tree
    /// </summary>
    private void DrawNodes()
    {
        if (GraphCanvas == null) return;
        GraphCanvas.Children.Clear();
        foreach (var node in NodePositions)
        {
            var (x, y) = node.Value;
            GetNodeView(node.Key).DrawNode(GraphCanvas, x, y);
        }

        DrawEdges();

        GraphCanvas.Width = NodePositions.Values.Max(p => p.x) + Settings.WidthPerNode + Settings.ExtraWidth;
        GraphCanvas.Height = NodePositions.Values.Max(p => p.y) + Settings.HeightPerNode + Settings.ExtraHeight;
    }

    /// <summary>
    /// Draw all the edges between the nodes
    /// </summary>
    private void DrawEdges()
    {
        if (GraphCanvas == null) return;
        foreach (var node in NodePositions)
        {
            var parentNode = node.Key;
            var (parentX, parentY) = node.Value;
            parentX += Settings.WidthPerNode / 2;
            parentY += Settings.HeightPerNode;

            foreach (var child in parentNode.Children)
            {
                if (NodePositions.ContainsKey(child))
                {
                    var (childX, childY) = NodePositions[child];
                    GetNodeView(child).DrawEdge(GraphCanvas, parentX, parentY,
                                              childX + Settings.WidthPerNode / 2, childY);
                }
            }
        }
    }
}