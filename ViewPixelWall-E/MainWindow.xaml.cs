using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ViewPixelWall_E.Interfaces;
using Canvas = System.Windows.Controls.Canvas;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace ViewPixelWall_E
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IPaint
    {
        private int canvasHeight;
        private int canvasWidth;
        private double rectSize;
        private double zoomFactor;
        private double lastValidZoomFactor;
        private Handlers handlers;
        public Rectangle[,] MainCanvasMatrix => throw new NotImplementedException();

        public WallE wallE => throw new NotImplementedException();

        public MainWindow()
        {
            InitializeComponent();
            handlers = new Handlers(this);
            rectSize = 10;
            zoomFactor = 1;
            UpdateLastValidZoomFactor(1);
            ShowCanvasSetupWindow();
        }

        private void UpdateMainCanvasSize()
        {
            MainCanvas.Height = canvasHeight;
            MainCanvas.Width = canvasWidth;
        }

        private void InitializeMainCanvas()
        {
            if (canvasHeight != 0 || canvasWidth != 0)
                return;

            MainCanvas.Children.Clear();

            for (int y = 0; y < canvasHeight; y++)
            {
                for (int x = 0; x < canvasWidth; x++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Height = 10,
                        Width = 10,
                        Fill = Brushes.Blue,
                    };
                    Canvas.SetLeft(rect, x * rectSize);
                    Canvas.SetTop(rect, y * rectSize);
                    MainCanvas.Children.Add(rect);
                }
            }
            MainCanvas.UpdateLayout();
        }

        private void ApplyZoom()
        {
            if (MainCanvas == null)
                return;
            if (MainCanvas.RenderTransform is ScaleTransform scaleTransform)
            {
                scaleTransform.ScaleX = zoomFactor;
                scaleTransform.ScaleY = zoomFactor;
            }
            else
            {
                MainCanvas.LayoutTransform = new ScaleTransform(zoomFactor, zoomFactor);
                MainCanvas.RenderTransformOrigin = new Point(0, 0);
            }

            UpdateLastValidZoomFactor(zoomFactor);

            MainCanvas.UpdateLayout();
            MainScrollViewer.UpdateLayout();
        }

        private void ShowCanvasSetupWindow()
        {
            CanvasSetupWindow canvasSetupWindow = new();

            bool? result = canvasSetupWindow.ShowDialog();
            if (result != true)
            {
                return;
            }
            canvasHeight = canvasSetupWindow.CanvasHeight;
            canvasWidth = canvasSetupWindow.CanvasWidth;

            UpdateMainCanvasSize();
        }

        private void ZoomTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            string[] subStrings = ZoomTextBox.Text.Split('%');
            string lastZoomFactorText = $"{lastValidZoomFactor * 100}";
            if (double.TryParse(subStrings[0], out double zoomPercent)
                || double.TryParse(ZoomTextBox.Text, out zoomPercent))
            {
                if (zoomPercent > 0)
                {
                    zoomFactor = zoomPercent * 0.01f;
                    ZoomTextBox.Text = zoomPercent.ToString() + "%";
                    ApplyZoom();
                }
                else
                {
                    MessageBox.Show("Zoom factor must be greater than 0.");
                    ZoomTextBox.Text = lastZoomFactorText + "%";
                }
            }
            else
            {
                MessageBox.Show("Invalid zoom factor.");
                ZoomTextBox.Text = lastZoomFactorText + "%";
            }
        }

        private void UpdateLastValidZoomFactor(double newZoomFactor) 
            => lastValidZoomFactor = newZoomFactor;

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e) 
            => Close();

        private void NewMenuItem_Click(object sender, RoutedEventArgs e) 
            => ShowCanvasSetupWindow();
    }
}