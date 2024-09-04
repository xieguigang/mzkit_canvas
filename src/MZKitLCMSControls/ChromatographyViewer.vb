Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.MIME.Html.CSS

Public Class ChromatographyViewer

    ''' <summary>
    ''' all Chromatography data points
    ''' </summary>
    Dim chromatography As ChromatogramTick()
    Dim plotPadding As Padding = $"padding: 100px 100px 200px 250px;"

    Public Property XLabel As String = "Rentention Time(s)"
    Public Property YLabel As String = "Intensity"

    ''' <summary>
    ''' title name of the Chromatography data
    ''' </summary>
    ''' <returns></returns>
    Public Property Title As String

    ''' <summary>
    ''' Other Chromatography data overlaps with current Chromatography data 
    ''' </summary>
    ''' <returns></returns>
    Public Property Overlaps As NamedCollection(Of ChromatogramTick)()

    Public ReadOnly Property ChromatographyPlot As Image
        Get
            Return PictureBox1.BackgroundImage
        End Get
    End Property

    Private Sub Rendering()
        Dim chromatography As TICplot
        Dim theme As New Theme With {.padding = plotPadding.ToString}
        Dim scale As Double = 2.5
        Dim size_str As String = $"{PictureBox1.Width * scale},{PictureBox1.Height * scale}"
        Dim title As String = Me.Title

        If title.StringEmpty(, True) Then
            title = "Chromatography Plot"
        End If

        If Overlaps.IsNullOrEmpty Then
            chromatography = New TICplot(New NamedCollection(Of ChromatogramTick)(Title, Me.chromatography), Nothing, 0, isXIC:=True, fillAlpha:=255, fillCurve:=False, labelLayoutTicks:=-1, bspline:=2, theme:=theme)
        Else
            Dim overlaps = _Overlaps.Join(New NamedCollection(Of ChromatogramTick)(Title, Me.chromatography))
            chromatography = New TICplot(overlaps, Nothing, 0, isXIC:=True, fillAlpha:=255, fillCurve:=False, labelLayoutTicks:=-1, bspline:=2, theme:=theme)
        End If

        chromatography.xlabel = XLabel
        chromatography.ylabel = YLabel

        PictureBox1.BackgroundImage = chromatography.Plot(size_str, ppi:=120).AsGDIImage
    End Sub

    Public Sub SetChromatography(data As IEnumerable(Of ChromatogramTick))
        chromatography = data.ToArray
        Rendering()
    End Sub

    Public Sub SetPlotPadding(padding As Padding)
        plotPadding = padding
    End Sub

    Private Sub PictureBox1_SizeChanged(sender As Object, e As EventArgs) Handles PictureBox1.SizeChanged
        Call Rendering()
    End Sub
End Class
