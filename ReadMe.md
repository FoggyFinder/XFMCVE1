### [ Android ] Changes to HeightRequest are ignored inside OnSizeAllocated ####

```fs
    override t.OnSizeAllocated(w, h) =
        base.OnSizeAllocated(w, h)
        let hr = w / float aspect
        System.Diagnostics.Debug.WriteLine($"W = {w}, H = {h}; hr = {hr}")
        t.HeightRequest <- hr
        t.InvalidateMeasure()
 ```       

#### Steps to replicate

1. Run app.
2. Tap on svg.
3. It changes to opposite and looks squeezed.
4. For all next taps rendering will be correct. 
