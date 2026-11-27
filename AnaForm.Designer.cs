namespace FlappyBird;

partial class AnaForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        oyunZamanlayici = new System.Windows.Forms.Timer(components);
        oyunPaneli = new OyunPaneli();
        SuspendLayout();
        //
        // oyunZamanlayici
        //
        oyunZamanlayici.Interval = 16; // ~60 kare/saniye
        oyunZamanlayici.Tick += oyunZamanlayici_Tick;
        //
        // oyunPaneli
        //
        oyunPaneli.Dock = DockStyle.Fill;
        oyunPaneli.Name = "oyunPaneli";
        oyunPaneli.TabIndex = 0;
        oyunPaneli.Paint += oyunPaneli_Paint;
        oyunPaneli.MouseDown += oyunPaneli_MouseDown;
        //
        // AnaForm
        //
        AutoScaleMode = AutoScaleMode.None;
        ClientSize = new Size(480, 640);
        Controls.Add(oyunPaneli);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        KeyPreview = true;
        MaximizeBox = false;
        Name = "AnaForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Flappy Bird — YMH219";
        Load += AnaForm_Load;
        KeyDown += AnaForm_KeyDown;
        Deactivate += AnaForm_Deactivate;
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Timer oyunZamanlayici;
    private OyunPaneli oyunPaneli;
}
