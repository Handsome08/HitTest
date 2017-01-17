using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HitTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        //单选
        private bool isDragging = false;
        private DrawingVisual selectedVisual;
        private Vector clickOffset;

        //多选
        private bool isMulti = false;
        private DrawingVisual selectionSquare;
        private Point selectionSquareTopLeft;
        private void drawingBoard_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pointClicked = e.GetPosition(drawingBoard);
            

            if (AddButton.IsChecked == true)
            {
                DrawingVisual visual = new DrawingVisual();
                DrawSquare(visual,pointClicked,false);
                drawingBoard.AddVisual(visual);
            }
            else if (DeleteButton.IsChecked == true)
            {
                DrawingVisual visual = drawingBoard.GetVisual(pointClicked);
                if(visual != null) drawingBoard.DeleteVisual(visual);
            }
            else if (SelectButton.IsChecked == true)
            {
                DrawingVisual visual = drawingBoard.GetVisual(pointClicked);

                if (visual != null)
                {
                    Point topLeftCorner = new Point(
                        visual.ContentBounds.Left + drawPen.Thickness / 2,
                        visual.ContentBounds.Top + drawPen.Thickness / 2);
                    DrawSquare(visual,topLeftCorner,true);
                    clickOffset = topLeftCorner - pointClicked;
                    isDragging = true;

                    selectedVisual = visual;
                }
            }
            else if(MultiSelect.IsChecked == true)
            {
                selectionSquare = new DrawingVisual();
                drawingBoard.AddVisual(selectionSquare);
                isMulti = true;
                selectionSquareTopLeft = pointClicked;
                drawingBoard.CaptureMouse();
            }
        }

        private void drawingBoard_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            if (isMulti)
            {
                RectangleGeometry geometry = new RectangleGeometry(
                    new Rect(selectionSquareTopLeft, e.GetPosition(drawingBoard)));
                List<DrawingVisual> visuals = drawingBoard.GetVisuals(geometry);
                MessageBox.Show(visuals.Count.ToString());
                isMulti = false;
                drawingBoard.DeleteVisual(selectionSquare);
                drawingBoard.ReleaseMouseCapture();
            }
        }

        private void drawingBoard_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPoint = e.GetPosition(drawingBoard) + clickOffset;
                DrawSquare(selectedVisual,currentPoint,true);
            }
            else if (isMulti)
            {
                this.DrawSelectionSquare(selectionSquareTopLeft,e.GetPosition(drawingBoard));
            }
        }

        //在Canvas中添加正方形所需变量和方法
        private Brush drawingBrush = Brushes.AliceBlue;
        private Brush seletedBrush = Brushes.LightGoldenrodYellow;
        private  Pen drawPen = new Pen(Brushes.SteelBlue,3);
        private Size squareSize = new Size(30,30);
        private void DrawSquare(DrawingVisual visual, Point topLeftCorner, bool IsSelected)
        {
            using (DrawingContext dc = visual.RenderOpen())
            {
                Brush brush = drawingBrush;
                if (IsSelected) brush = seletedBrush;
                dc.DrawRectangle(brush,drawPen,new Rect(topLeftCorner,squareSize));
            }
        }

        //添加选择框
        private Pen selectionSquarePen = new Pen(Brushes.Black,2);
        private Brush selectionSquareBrush = Brushes.Transparent;

        public void DrawSelectionSquare(Point point1, Point point2)
        {
            selectionSquarePen.DashStyle = DashStyles.Dash;
            using (DrawingContext dc = selectionSquare.RenderOpen())
            {
                dc.DrawRectangle(selectionSquareBrush,selectionSquarePen,new Rect(point1,point2));
            }
        }
    }
}
