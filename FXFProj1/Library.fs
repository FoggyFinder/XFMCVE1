namespace FXFProj1

open Xamarin.Forms
open SkiaSharp.Views.Forms
open SkiaSharp
open System
open Svg.Skia

type SvgV(svg:string, color:SKColor) =
    inherit SKCanvasView()
    
    let skSvg = new SKSvg()
    let skPicture = skSvg.FromSvg(svg)
    let pwidth, pheight = skPicture.CullRect.Width, skPicture.CullRect.Height
    let aspect = pwidth / pheight
    let skPaint = new SKPaint(ColorFilter = 
        SKColorFilter.CreateBlendMode(color, SKBlendMode.SrcIn))

    override t.OnSizeAllocated(w, h) =
        base.OnSizeAllocated(w, h)
        let hr = w / float aspect
        System.Diagnostics.Debug.WriteLine($"W = {w}, H = {h}; hr = {hr}")
        t.HeightRequest <- hr
        t.InvalidateMeasure()

    override t.OnPaintSurface(args) =
        base.OnPaintSurface args
        let canvas = args.Surface.Canvas
        canvas.Clear()
        let width, height = canvas.DeviceClipBounds.Width, canvas.DeviceClipBounds.Height
        let wscale, hscale = float32 width / pwidth, float32 height / pheight
        let mutable matrix = SkiaSharp.SKMatrix.CreateScale(wscale, hscale)
        canvas.DrawPicture(skPicture, ref matrix, skPaint)

    interface IDisposable with 
        member t.Dispose() = 
            (skPaint :> IDisposable).Dispose()
            (skPicture :> IDisposable).Dispose()
            (skSvg :> IDisposable).Dispose()
        

type SomeView(isLeft:bool) as self =
    inherit ContentView(VerticalOptions = LayoutOptions.Center, 
                HorizontalOptions = LayoutOptions.Center)
    let mutable state = isLeft

    // https://icons.getbootstrap.com/icons/chevron-left/
    let leftSvg = """
<svg fill="#FFFFFF" viewBox="0 0 24 20">
   <path fill-rule="evenodd" d="M11.354 1.646a.5.5 0 0 1 0 .708L5.707 8l5.647 5.646a.5.5 0 0 1-.708.708l-6-6a.5.5 0 0 1 0-.708l6-6a.5.5 0 0 1 .708 0z"/>
</svg>"""

    // https://icons.getbootstrap.com/icons/chevron-right/
    let rightSvg = """
<svg fill="#FFFFFF" viewBox="0 0 24 20">
  <path fill-rule="evenodd" d="M4.646 1.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1 0 .708l-6 6a.5.5 0 0 1-.708-.708L10.293 8 4.646 2.354a.5.5 0 0 1 0-.708z"/>
</svg>"""

    let tgr = TapGestureRecognizer()
    let leftV, rightV =
        let getSvg isLeft =
            let svg = if isLeft then leftSvg else rightSvg
            let color = if isLeft then SKColors.Green else SKColors.Red
            let svgV = new SvgV(svg, color, WidthRequest = 220) 
            svgV.GestureRecognizers.Add tgr
            svgV
        lazy(getSvg true), lazy(getSvg false)

    let update() = 
        self.Content <- if state then leftV.Value else rightV.Value

    do tgr.Tapped.Add(fun _ -> state <- not state; update())
       update()

    interface IDisposable with 
        member t.Dispose() = 
            if leftV.IsValueCreated then
                (leftV.Value :> IDisposable).Dispose()
            if rightV.IsValueCreated then
                (rightV.Value :> IDisposable).Dispose()

type App() =
    inherit Application()
    let tv1 = new SomeView(true)
    let tv2 = new SomeView(false)

    let sL = StackLayout(VerticalOptions = LayoutOptions.Center)
    do sL.Children.Add(tv1)
    do sL.Children.Add(tv2)

    do base.MainPage <- ContentPage(Content = sL)

    interface IDisposable with 
        member t.Dispose() = 
            (tv1 :> IDisposable).Dispose()
            (tv2 :> IDisposable).Dispose()