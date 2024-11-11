<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class spectrumViewerForm1
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.MassSpectrometryViewer1 = New MZKitLCMSControls.MassSpectrometryViewer()
        Me.SuspendLayout()
        '
        'MassSpectrometryViewer1
        '
        Me.MassSpectrometryViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MassSpectrometryViewer1.Location = New System.Drawing.Point(0, 0)
        Me.MassSpectrometryViewer1.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.MassSpectrometryViewer1.MassRange = Nothing
        Me.MassSpectrometryViewer1.Name = "MassSpectrometryViewer1"
        Me.MassSpectrometryViewer1.Size = New System.Drawing.Size(800, 450)
        Me.MassSpectrometryViewer1.Spectrum = Nothing
        Me.MassSpectrometryViewer1.TabIndex = 0
        Me.MassSpectrometryViewer1.Title = Nothing
        '
        'spectrumViewerForm1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.MassSpectrometryViewer1)
        Me.Name = "spectrumViewerForm1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents MassSpectrometryViewer1 As MZKitLCMSControls.MassSpectrometryViewer
End Class
