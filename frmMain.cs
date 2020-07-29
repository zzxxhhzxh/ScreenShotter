using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;

namespace ScreenShotter
{
    public partial class frmMain : Form
    {
        Bitmap img, shotImg, cache;
        Rectangle shotRect;
        Point p; // start point
        readonly Pen dPen = Pens.DodgerBlue;
        readonly Brush dBrush = new SolidBrush(Color.FromArgb(63, 31, 63, 63));


        public frmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            var screenBounds = Screen.AllScreens[Screen.AllScreens.Length - 1].Bounds;
            Bounds = screenBounds;
            img = new Bitmap(screenBounds.Width, screenBounds.Height);
            using (var g = Graphics.FromImage(img))
            {
                g.CopyFromScreen(0, 0, 0, 0, screenBounds.Size);
            }
            cache = new Bitmap(img);
        }

        private void FrmMain_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(img, 0, 0);
        }

        private void FrmMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
                Application.Exit();
        }

        private void FrmMain_MouseDown(object sender, MouseEventArgs e)
        {
            p = e.Location;
            MouseMove += new MouseEventHandler(FrmMain_MouseMove);
        }

        private void FrmMain_MouseMove(object sender, MouseEventArgs e)
        {
            img = new Bitmap(cache);
            shotRect = new Rectangle(p.X, p.Y, e.X - p.X, e.Y - p.Y);
            using (var g = Graphics.FromImage(img))
            {
                g.CompositingMode = CompositingMode.SourceOver;
                g.FillRectangle(dBrush, shotRect);
                g.DrawRectangle(dPen, shotRect);
            }
            Refresh();
        }

        private void FrmMain_MouseUp(object sender, MouseEventArgs e)
        {
            MouseMove -= new MouseEventHandler(FrmMain_MouseMove);
            img = new Bitmap(cache);
            Refresh();
            if (shotRect.Width == 0 || shotRect.Height == 0)
                return;
            Cursor = Cursors.Default;
            GetScreenShot(shotRect);
            shotRect = new Rectangle();
            Cursor = Cursors.Cross;
        }

        private void GetScreenShot(Rectangle rect)
        {
            shotImg = img.Clone(rect, PixelFormat.DontCare);
            Clipboard.SetImage(shotImg);
            string name = $"Screenshot_{DateTime.Now:MMddyy_HHmmss}.png";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            shotImg.Save(Path.Combine(path, name), ImageFormat.Png);
        }
    }
}