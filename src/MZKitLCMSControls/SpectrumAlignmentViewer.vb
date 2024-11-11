Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml

Public Class SpectrumAlignmentViewer

    Public Property alignment As AlignmentOutput
        Get
            Return m_alignment
        End Get
        Set(value As AlignmentOutput)
            m_alignment = value
            Rendering()
        End Set
    End Property

    Dim m_alignment As AlignmentOutput

    Private Sub Rendering()

    End Sub

End Class
