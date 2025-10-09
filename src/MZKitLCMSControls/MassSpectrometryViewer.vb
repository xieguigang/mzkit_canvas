#Region "Microsoft.VisualBasic::cf9e16c2925612d0f681491fcee3d426, mzkit\ux\Canvas\src\MZKitLCMSControls\MassSpectrometryViewer.vb"

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

    '   Total Lines: 148
    '    Code Lines: 120 (81.08%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 28 (18.92%)
    '     File Size: 5.25 KB


    ' Class MassSpectrometryViewer
    ' 
    '     Properties: MassRange, Spectrum, SpectrumPlot, Title
    ' 
    '     Sub: MassSpectrometryViewer_SizeChanged, PictureBox1_MouseMove, PictureBox1_Paint, Rendering, (+2 Overloads) SetSpectrum
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.MIME.Html.CSS

Public Class MassSpectrometryViewer

    Public Property MassRange As DoubleRange
        Get
            Return m_massrange
        End Get
        Set(value As DoubleRange)
            m_massrange = value
            Rendering()
        End Set
    End Property

    Public Property Title As String
        Get
            Return m_title
        End Get
        Set(value As String)
            m_title = value
            Rendering()
        End Set
    End Property

    Public Property Spectrum As ms2()
        Get
            Return m_spectrum
        End Get
        Set(value As ms2())
            m_spectrum = value
            Rendering()
        End Set
    End Property

    Dim m_title As String
    Dim m_spectrum As ms2()
    Dim m_massrange As DoubleRange
    Dim m_theme As New Theme

    Public ReadOnly Property SpectrumPlot As Image
        Get
            Return PictureBox1.BackgroundImage
        End Get
    End Property

    Dim scaleFactor As Single = 1.125
    Dim scaleX As d3js.scale.LinearScale
    Dim scaleY As d3js.scale.LinearScale
    Dim paddingLayout As PaddingLayout

    Dim scaleMz As d3js.scale.LinearScale
    Dim scaleInto As d3js.scale.LinearScale

    Private Sub Rendering()
        If m_spectrum Is Nothing Then
            Return
        End If
        If m_massrange Is Nothing Then
            Return
        End If

        Dim size As New Size(Width * scaleFactor, Height * scaleFactor)
        Dim paddingCss = CType(m_theme.padding, Padding)
        Dim css As New CSSEnvirnment(size)
        Dim region As New GraphicsRegion(paddingCss, size)
        Dim rect = region.PlotRegion(css)

        paddingLayout = PaddingLayout.EvaluateFromCSS(css, paddingCss)

        Dim realX = paddingLayout.Left / scaleFactor
        Dim realY = paddingLayout.Top / scaleFactor

        scaleX = d3js.scale.linear().domain(values:={realX, rect.Width / scaleFactor + realX}).range(paddingLayout.Left, rect.Width + paddingLayout.Left)
        scaleY = d3js.scale.linear().domain(values:={realY, rect.Height / scaleFactor + realY}).range(paddingLayout.Top, rect.Height + paddingLayout.Top)
        scaleMz = d3js.scale.linear.domain(values:={paddingLayout.Left, rect.Width + paddingLayout.Left}).range(values:=PeakAssign.CreateXMzAxisTicks(m_spectrum))
        scaleInto = d3js.scale.linear.domain(values:={paddingLayout.Top, rect.Height + paddingLayout.Top}).range(values:={0, 110})

        Using g As Graphics2D = size.CreateGDIDevice(BackColor)
            Dim canvas As New PeakAssign(Title, m_spectrum, "red", 0.3, m_theme)

            Call canvas.Plot(g, region)
            Call g.Flush()

            PictureBox1.BackgroundImage = g.ImageResource
        End Using
    End Sub

    Public Sub SetSpectrum(m As LibraryMatrix)
        m_title = m.name
        m_spectrum = m.Array
        m_massrange = m_spectrum.Select(Function(mi) mi.mz).Range

        Call Rendering()
    End Sub

    Public Sub SetSpectrum(m As PeakMs2)
        m_title = m.lib_guid
        m_spectrum = m.mzInto
        m_massrange = m_spectrum.Select(Function(mi) mi.mz).Range

        Call Rendering()
    End Sub

    Private Sub MassSpectrometryViewer_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        Call Rendering()
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        Dim fp = PictureBox1.PointToClient(Cursor.Position)
        Dim mz = scaleX(fp.X) - paddingLayout.Left
        Dim into = scaleY(fp.Y) - paddingLayout.Top

        If mz < 0 OrElse into < 0 Then
            Call ToolTip1.SetToolTip(PictureBox1, Nothing)
        Else
            mz = scaleMz(mz)
            into = scaleInto(into)

            Call ToolTip1.SetToolTip(PictureBox1, $"m/z {mz.ToString("F4")} [{into}%]")
        End If
    End Sub

    Private Sub PictureBox1_Paint(sender As Object, e As PaintEventArgs) Handles PictureBox1.Paint
        Dim fp = PictureBox1.PointToClient(Cursor.Position)

        If scaleX Is Nothing OrElse scaleY Is Nothing Then
            Return
        End If

        Dim mz = scaleX(fp.X) - paddingLayout.Left
        Dim into = scaleY(fp.Y) - paddingLayout.Top

        If mz < 0 OrElse into < 0 Then
        Else
            Dim size As New Size(Width * scaleFactor, Height * scaleFactor)
            Dim paddingCss = CType(m_theme.padding, Padding)
            Dim css As New CSSEnvirnment(size)
            Dim region As New GraphicsRegion(paddingCss, size)
            Dim rect = region.PlotRegion(css)

            Call e.Graphics.DrawLine(Pens.Red, paddingLayout.Left / scaleFactor, fp.Y, paddingLayout.Left / scaleFactor + rect.Width / scaleFactor, fp.Y)

        End If
    End Sub
End Class
