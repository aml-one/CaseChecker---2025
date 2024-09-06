using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.IO;

namespace CaseChecker.MVVM.Core;

public class ImageViewControl : Border
{
    private Point origin;
    private Point start;
    private Image image;

    public ImageViewControl()
    {
        ClipToBounds = true;
        Loaded += OnLoaded;
        image = new Image
        {
            //IsManipulationEnabled = true,
            RenderTransformOrigin = new Point(0.5, 0.5),
            RenderTransform = new TransformGroup
            {
                Children =
                    [
                        new ScaleTransform(),
                        new TranslateTransform()
                    ]
            }
        };
    }

    #region ImagePath

    /// <summary>
    ///     ImagePath Dependency Property
    /// </summary>
    public static readonly DependencyProperty ImagePathProperty = DependencyProperty.Register("ImagePath", typeof(string), typeof(ImageViewControl), new FrameworkPropertyMetadata(string.Empty, OnImagePathChanged));

    /// <summary>
    ///     Gets or sets the ImagePath property. This dependency property 
    ///     indicates the path to the image file.
    /// </summary>
    public string ImagePath
    {
        get { return (string)GetValue(ImagePathProperty); }
        set { SetValue(ImagePathProperty, value); }
    }

    /// <summary>
    ///     Handles changes to the ImagePath property.
    /// </summary>
    private static void OnImagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (ImageViewControl)d;
        var oldImagePath = (string)e.OldValue;
        var newImagePath = target.ImagePath;
        target.ReloadImage(newImagePath);
        target.OnImagePathChanged(oldImagePath, newImagePath);
    }

    /// <summary>
    ///     Provides derived classes an opportunity to handle changes to the ImagePath property.
    /// </summary>
    protected virtual void OnImagePathChanged(string oldImagePath, string newImagePath)
    {
    }

    #endregion


    #region ImageSource

    /// <summary>
    ///     ImageSource Dependency Property
    /// </summary>
    public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(ImageViewControl), new FrameworkPropertyMetadata(string.Empty, OnImageSourceChanged));

    /// <summary>
    ///     Gets or sets the ImageSource property. This dependency property 
    ///     indicates the path to the image file.
    /// </summary>
    public string ImageSource
    {
        get { return (string)GetValue(ImageSourceProperty); }
        set { SetValue(ImageSourceProperty, value); }
    }

    /// <summary>
    ///     Handles changes to the ImageSource property.
    /// </summary>
    private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (ImageViewControl)d;
        var oldImageSource = (string)e.OldValue;
        var newImageSource = target.ImageSource;
        target.ReloadImageSource(newImageSource);
        target.OnImageSourceChanged(oldImageSource, newImageSource);
    }

    /// <summary>
    ///     Provides derived classes an opportunity to handle changes to the ImageSource property.
    /// </summary>
    protected virtual void OnImageSourceChanged(string oldImageSource, string newImageSource)
    {
    }

    #endregion

    private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
    {

        // NOTE I use a border as the first child, to which I add the image. I do this so the panned image doesn't partly obscure the control's border.
        // In case you are going to use rounder corner's on this control, you may to update your clipping, as in this example:
        // http://wpfspark.wordpress.com/2011/06/08/clipborder-a-wpf-border-that-clips/
        var border = new Border
        {
            IsManipulationEnabled = true,
            ClipToBounds = true,
            Child = image
        };
        Child = border;

        image.MouseWheel += (s, e) =>
        {
            var zoom = e.Delta > 0 ? .2 : -.2;
            var position = e.GetPosition(image);
            image.RenderTransformOrigin = new Point(position.X / image.ActualWidth, position.Y / image.ActualHeight);
            var st = (ScaleTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is ScaleTransform);

            if (image.ActualWidth * (st.ScaleX + zoom) < 200 || image.ActualHeight * (st.ScaleY + zoom) < 200) //don't zoom out too small.
                return;

            st.ScaleX += zoom;
            st.ScaleY += zoom;
            e.Handled = true;
        };

        image.MouseRightButtonDown += (s, e) =>
        {
            if (e.ClickCount == 2)
                ResetPanZoom();
            else
            {
                image.CaptureMouse();
                var tt = (TranslateTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is TranslateTransform);
                start = e.GetPosition(this);
                origin = new Point(tt.X, tt.Y);
            }
            e.Handled = true;
        };

        image.MouseMove += (s, e) =>
        {
            if (!image.IsMouseCaptured) return;
            var tt = (TranslateTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is TranslateTransform);
            var v = start - e.GetPosition(this);
            tt.X = origin.X - v.X;
            tt.Y = origin.Y - v.Y;
            e.Handled = true;
        };

        image.MouseRightButtonUp += (s, e) => image.ReleaseMouseCapture();

        //NOTE I apply the manipulation to the border, and not to the image itself (which caused stability issues when translating)!
        border.ManipulationDelta += (o, e) =>
        {
            var st = (ScaleTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is ScaleTransform);
            var tt = (TranslateTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is TranslateTransform);

            st.ScaleX *= e.DeltaManipulation.Scale.X;
            st.ScaleY *= e.DeltaManipulation.Scale.X;
            tt.X += e.DeltaManipulation.Translation.X;
            tt.Y += e.DeltaManipulation.Translation.Y;

            e.Handled = true;
        };
    }

    private void ResetPanZoom()
    {
        var st = (ScaleTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is ScaleTransform);
        var tt = (TranslateTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is TranslateTransform);
        st.ScaleX = st.ScaleY = 1;
        tt.X = tt.Y = 0;
        image.RenderTransformOrigin = new Point(0.5, 0.5);
    }

    /// <summary>
    /// Load the image (and do not keep a hold on it, so we can delete the image without problems)
    /// </summary>
    /// <param name="path"></param>
    private void ReloadImage(string path)
    {
        try
        {
            ResetPanZoom();
            // load the image, specify CacheOption so the file is not locked
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();
            image.Source = bitmapImage;
        }
        catch (SystemException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    /// <summary>
    /// Load the image from base64 string
    /// </summary>
    /// <param name="stream"></param>
    private void ReloadImageSource(string stream)
    {
        try
        {
            ResetPanZoom();
            var bitmapImage = new BitmapImage();
            using (var mem = new MemoryStream(Convert.FromBase64String(stream)))
            {
                mem.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = null;
                bitmapImage.StreamSource = mem;
                bitmapImage.EndInit();
            }

            image.Source = bitmapImage;
        }
        catch (SystemException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
