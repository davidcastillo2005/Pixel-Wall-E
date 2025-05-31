using System.Windows;
using System.Windows.Input;

namespace ViewPixelWall_E
{
    /// <summary>
    /// Interaction logic for CanvasSetupWindow.xaml
    /// </summary>
    public partial class CanvasSetupWindow : Window
    {
        public int CanvasHeight { get; private set; }
        public int CanvasWidth { get; private set; }

        public CanvasSetupWindow()
        {
            InitializeComponent();
            CanvasWidth = 0;
            CanvasHeight = 0;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (CanvasHeight == 0 || CanvasWidth == 0)
            {
                MessageBox.Show("Invalid width or invalid height.");
                return;
            }
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(Object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void UpdatePreviewCanvas(double height, double width)
        {
            if (PreviewBorder == null
                || PreviewCanvas == null
                || width == 0
                || height == 0)
                return;

            double aspectRatio = (double)height / width;
            double newHeight, newWidth;
            if (aspectRatio >= 1)
            {
                newHeight = GetPreviewMaxSize();
                newWidth = GetPreviewMaxSize() / aspectRatio;
            }
            else
            {
                newWidth = GetPreviewMaxSize();
                newHeight = GetPreviewMaxSize() * aspectRatio;
            }

            PreviewCanvas.Height = newHeight;
            PreviewCanvas.Width = newWidth;
        }

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
            => UpdatePreviewCanvas(GetPreviewMaxSize(), GetPreviewMaxSize());

        private double GetPreviewMaxSize() => PreviewBorder.ActualHeight * 0.75;

        private void HeightTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string str = HeightTextBox.Text;
            if (!int.TryParse(str, out int height)
                || height <= 0)
            {
                MessageBox.Show("Invalid height.");
                return;
            }
            CanvasHeight = height;
            UpdatePreviewCanvas(CanvasHeight, CanvasWidth);
        }

        private void WidthTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string str = WidthTextBox.Text;
            if (!int.TryParse(str, out int width)
                || width <= 0)
            {
                MessageBox.Show("Invalid width.");
                return;
            }
            CanvasWidth = width;
            UpdatePreviewCanvas(CanvasHeight, CanvasWidth);
        }
    }
}
