using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OrderReader.Pages.Settings
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = ClippingGrid.ActualWidth;
            double height = ClippingGrid.ActualHeight;

            // Update the Path's points to match the size of the Grid
            ClipPathLeft.Data = new PathGeometry
            {
                Figures = new PathFigureCollection
                {
                    new PathFigure
                    {
                        StartPoint = new Point(0, 0),
                        Segments = new PathSegmentCollection
                        {
                            new LineSegment(new Point(width+1, 0), true),
                            new LineSegment(new Point(0, height+1), true)
                        }
                    }
                }
            };

            // Update the Path's points to match the size of the Grid
            ClipPathRight.Data = new PathGeometry
            {
                Figures = new PathFigureCollection
                {
                    new PathFigure
                    {
                        StartPoint = new Point(width, -1),
                        Segments = new PathSegmentCollection
                        {
                            new LineSegment(new Point(width, height), true),
                            new LineSegment(new Point(-1, height), true)
                        }
                    }
                }
            };
        }
    }
}
