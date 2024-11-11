﻿#Region "Microsoft.VisualBasic::38bfe84ae130e4f357215f58404dcacb, mzkit\ux\Canvas\src\MZKitLCMSControls\MassSpectrometryViewer.vb"

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

'   Total Lines: 3
'    Code Lines: 2 (66.67%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 1 (33.33%)
'     File Size: 50 B


' Class MassSpectrometryViewer
' 
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Drawing
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver

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

    Private Sub Rendering()
        If m_spectrum Is Nothing Then
            Return
        End If
        If m_massrange Is Nothing Then
            Return
        End If

        Dim scale As Double = 1.5
        Dim size As New Size(Width * scale, Height * scale)

        Using g As Graphics2D = size.CreateGDIDevice(BackColor)
            Dim canvas As New PeakAssign(Title, m_spectrum, "red", 0.3, m_theme)
            Dim region As New GraphicsRegion(m_theme.padding, size)

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

End Class

