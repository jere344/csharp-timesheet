using System.Windows;
using System.Windows.Controls;
using System;
using wisecorp.ViewModels;
using System.Windows.Navigation;
using System.Threading;
using System.Globalization;
using wisecorp.Models.DBModels;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;

namespace wisecorp.Views;

public partial class ViewManageProjects : Page
{
    public ViewManageProjects()
    {
        InitializeComponent();

        DataContext = new VMManageProjects();

        this.Loaded += Page_Loaded;
    }
    

    /// <summary>
    /// Set the title and size of the window
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ((MainWindow)Application.Current.MainWindow).SetWindowInfos((string)Application.Current.FindResource("manageProjects"), 800, 1100);
        var vm = (VMManageProjects)DataContext;
        vm.ZoomableCanvasControl = zoomableCanvasControl;
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // just clear the selected project when changing tabs
        var vm = (VMManageProjects)DataContext;
        vm.SelectedProject = null;
    }


    // When the user clicks outside of the treeview, deselect the selected project
    private void Page_MouseUp(object sender, MouseButtonEventArgs e)
    {
        // Get the element that was clicked
        var clickedElement = e.OriginalSource as DependencyObject;


        // Traverse up the visual tree to see if a TreeViewItem was clicked or if we are on the DockPanelSelectedItem
        while (clickedElement != null && clickedElement is not TreeViewItem && (clickedElement is not DockPanel || ((DockPanel)clickedElement).Name != "DockPanelSelectedItem"))
        {
            clickedElement = VisualTreeHelper.GetParent(clickedElement);
        }

        // If no TreeViewItem was found, it means the click was outside of any item
        if (clickedElement == null)
        {
            var vm = (VMManageProjects)DataContext;
            vm.SelectedProject = null;

            // Get the currently selected TreeViewItem
            if (ProjectsTreeView.ItemContainerGenerator.ContainerFromItem(ProjectsTreeView.SelectedItem) is TreeViewItem selectedItem)
            {
                selectedItem.IsSelected = false;
            }
            ProjectsTreeView.Focus();
        }
    }


    #region Tree view specific and drag and drop
    private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (e.NewValue is Project project)
        {
            var vm = (VMManageProjects)DataContext;
            vm.SelectedProject = project;
        }
    }
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    private void Project_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var project = (Project)((Grid)sender).DataContext;
            DragDrop.DoDragDrop((Grid)sender, new ProjectDto { Id = project.Id, Name = project.Name }, DragDropEffects.Move);
        }
    }

    private void Project_Drop(object sender, DragEventArgs e)
    {
        var projectDto = (ProjectDto)e.Data.GetData(typeof(ProjectDto));
        var project = ((VMManageProjects)DataContext).Context.Projects.Find(projectDto.Id);

        var newParent = (Project)((Grid)sender).DataContext;

        if (project == null || newParent == null)
        {
            return;
        }

        if (project.Id == newParent.Id) // Can't move a project into itself
        {
            return;
        }

        var vm = (VMManageProjects)DataContext;
        vm.MoveProject(project, newParent);
        e.Handled = true;
    }

    private void TreeView_Drop(object sender, DragEventArgs e)
    {
        if (!(e.OriginalSource is FrameworkElement element && element.DataContext is Project))
        {
            var projectDto = (ProjectDto)e.Data.GetData(typeof(ProjectDto));
            var project = ((VMManageProjects)DataContext).Context.Projects.Find(projectDto.Id);

            if (project == null)
            {
                return;
            }

            var vm = (VMManageProjects)DataContext;
            vm.MoveProject(project, null);
            e.Handled = true;
        }  
    }

    private void TreeViewItem_DragOver(object sender, DragEventArgs e)
    {
        if (sender is TreeViewItem item)
        {
            item.Background = Brushes.LightGray; // Change to a hover color
        }
    }

    private void TreeViewItem_DragLeave(object sender, DragEventArgs e)
    {
        if (sender is TreeViewItem item)
        {
            item.Background = Brushes.Transparent; // Revert to original color
        }
    }
    #endregion
}