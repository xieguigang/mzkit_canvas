Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Public Class spectrumViewerForm1
    Private Sub spectrumViewerForm1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim demo As New LibraryMatrix("test spectrum", {
            New ms2(53, 100),
            New ms2(129, 33)
        })

        Call MassSpectrometryViewer1.SetSpectrum(demo)
    End Sub
End Class
