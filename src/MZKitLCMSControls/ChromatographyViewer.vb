#Region "Microsoft.VisualBasic::62ae0d0baf7bee3997b37fce255d0224, mzkit\ux\Canvas\src\MZKitLCMSControls\ChromatographyViewer.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 139
    '    Code Lines: 100 (71.94%)
    ' Comment Lines: 12 (8.63%)
    '    - Xml Docs: 91.67%
    ' 
    '   Blank Lines: 27 (19.42%)
    '     File Size: 5.31 KB


    ' Class ChromatographyViewer
    ' 
    '     Properties: ChromatographyData, ChromatographyPlot, Overlaps, Title, XLabel
    '                 YLabel
    ' 
    '     Sub: PictureBox1_MouseMove, PictureBox1_SizeChanged, Rendering, SetChromatography, SetPlotPadding
    '          TranslateViewLocation
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.MIME.Html.CSS

Public Class ChromatographyViewer

    ''' <summary>
    ''' all Chromatography data points
    ''' </summary>
    Dim chromatography As ChromatogramTick()
    Dim plotPadding As Padding = $"padding: 25px 25px 100px 100px;"
    Dim scaleFactor As Double = 2

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

    Public ReadOnly Property ChromatographyData As ChromatogramTick()
        Get
            Return chromatography.ToArray
        End Get
    End Property

    Public ReadOnly Property ChromatographyPlot As Image
        Get
            Return PictureBox1.BackgroundImage
        End Get
    End Property

    Private Sub Rendering()
        Dim chromatography As TICplot
        Dim theme As New Theme With {
            .padding = plotPadding.ToString,
            .drawLabels = True,
            .tagCSS = "font-style: normal; font-size: 24; font-family: " & FontFace.BookmanOldStyle & ";",
            .tagColor = "red"
        }
        Dim size_str As String = $"{PictureBox1.Width * scaleFactor},{PictureBox1.Height * scaleFactor}"
        Dim title As String = Me.Title

        If Me.chromatography.IsNullOrEmpty Then
            Return
        End If

        If title.StringEmpty(, True) Then
            title = "Chromatography Plot"
        End If

        If Overlaps.IsNullOrEmpty Then
            chromatography = New TICplot(New NamedCollection(Of ChromatogramTick)(title, Me.chromatography), Nothing, 0, isXIC:=True, fillAlpha:=255, fillCurve:=False, labelLayoutTicks:=-1, bspline:=2, theme:=theme)
        Else
            Dim overlaps = _Overlaps.Join(New NamedCollection(Of ChromatogramTick)(title, Me.chromatography))
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

    Private Sub TranslateViewLocation(ByRef x As Integer, ByRef y As Integer, ByRef rt As Double, ByRef intensity As Double)
        If PictureBox1.BackgroundImage Is Nothing Then
            Return
        End If

        Dim clientXy = PictureBox1.PointToClient(Cursor.Position)
        Dim canvas As Size = PictureBox1.BackgroundImage.Size
        Dim viewClient As New GraphicsRegion(canvas, plotPadding)

        x = clientXy.X * scaleFactor
        y = clientXy.Y * scaleFactor

        ' translate to RT,intensity
        Dim view As Rectangle = viewClient.PlotRegion(New CSSEnvirnment(viewClient.Size))
        Dim xaxis = d3js.scale.linear.domain(values:=New Double() {view.Left, view.Right}).range(values:=chromatography.TimeArray.CreateAxisTicks)
        Dim yaxis = d3js.scale.linear.domain(values:=New Double() {view.Top, view.Bottom}).range(values:=chromatography.IntensityArray.CreateAxisTicks)

        If x < view.Left Then
            x = view.Left
        ElseIf x > view.Right Then
            x = view.Right
        End If
        If y < view.Top Then
            y = view.Top
        ElseIf y > view.Bottom Then
            y = view.Bottom
        End If

        rt = xaxis(x)
        intensity = yaxis(x)
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        Dim x, y As Integer
        Dim rt, into As Double

        Return

        Call TranslateViewLocation(x, y, rt, into)

        Using g As Graphics = PictureBox1.CreateGraphics
            Dim a As New Point(x, plotPadding.Top)
            Dim b As New Point(x, PictureBox1.Height * scaleFactor - plotPadding.Bottom)

            Call g.DrawLine(Pens.Red, a, b)
            Call g.DrawString($"RT: {(rt / 60).ToString("F1")}min, intensity: {into.ToString("G4")}", New Font(FontFace.SegoeUI, 16), Brushes.Red, New PointF(x, y))
        End Using
    End Sub
End Class
