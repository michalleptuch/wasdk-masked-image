using System;

using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;

namespace ImageWithMask
{
  public sealed partial class MainWindow : Window
  {
    private Compositor _compositor;
    private LoadedImageSurface _backgroundSurface;
    private LoadedImageSurface _maskSurface;

    public MainWindow()
    {
      InitializeComponent();

      AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
      AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
      AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

      MaskedImageGrid.Loaded += (_, _) => LoadBackground();
      MaskedImageGrid.Unloaded += (_, _) => UnloadBackground();
    }

    private void LoadBackground()
    {
      if (_compositor is null)
      {
        _compositor = Compositor;
      }

      //// Load images only once
      if (_backgroundSurface is null)
      {
        _backgroundSurface = LoadedImageSurface.StartLoadFromUri(new Uri("ms-appx:///Images/Background.jpg"));
      }

      if (_maskSurface is null)
      {
        //// Use Mask2.png to increase mask intense
        _maskSurface = LoadedImageSurface.StartLoadFromUri(new Uri("ms-appx:///Images/Mask.png"));
      }

      var imageBrush = _compositor.CreateSurfaceBrush(_backgroundSurface);
      imageBrush.Stretch = CompositionStretch.UniformToFill; //// Preserves image ratio and fills entire container (stack panel)

      var maskBrush = _compositor.CreateSurfaceBrush(_maskSurface);
      maskBrush.Stretch = CompositionStretch.Fill; //// Stretches mask image to the size of container

      var maskedBrush = _compositor.CreateMaskBrush();
      maskedBrush.Source = imageBrush;
      maskedBrush.Mask = maskBrush;

      var size = MaskedImageGrid.ActualSize;

      var outputSpriteVisual = _compositor.CreateSpriteVisual();
      outputSpriteVisual.Size = size;
      outputSpriteVisual.Brush = maskedBrush;

      ElementCompositionPreview.SetElementChildVisual(MaskedImageGrid, outputSpriteVisual);
    }

    private void UnloadBackground()
    {
      _backgroundSurface?.Dispose();
      _maskSurface?.Dispose();
    }

    private void RefreshBackground(object sender, SizeChangedEventArgs e)
    {
      LoadBackground();
    }
  }
}