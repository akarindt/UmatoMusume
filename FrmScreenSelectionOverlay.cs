namespace UmatoMusume
{
	public class FrmScreenSelectionOverlay : Form
	{
		public Rectangle SelectedRectangle { get; private set; }
		private Point _startPoint;
		private Point _endPoint;
		private bool _dragging = false;
		private Rectangle _overlayBounds;

		public FrmScreenSelectionOverlay() : this(SystemInformation.VirtualScreen) { }

		public FrmScreenSelectionOverlay(Rectangle _bounds)
		{
			_overlayBounds = _bounds;
			FormBorderStyle = FormBorderStyle.None;
			ShowInTaskbar = false;
			TopMost = true;
			BackColor = Color.White;
			Opacity = 0.2;
			DoubleBuffered = true;
			Cursor = Cursors.Cross;
			Bounds = _overlayBounds;
			StartPosition = FormStartPosition.Manual;
			KeyPreview = true;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				DialogResult = DialogResult.Cancel;
				Close();
			}
			base.OnKeyDown(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				_dragging = true;
				_startPoint = new Point(e.X + _overlayBounds.Left, e.Y + _overlayBounds.Top);
				_endPoint = _startPoint;
				Invalidate();
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (_dragging)
			{
				_endPoint = new Point(e.X + _overlayBounds.Left, e.Y + _overlayBounds.Top);
				Invalidate();
			}
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (_dragging && e.Button == MouseButtons.Left)
			{
				_dragging = false;
				_endPoint = new Point(e.X + _overlayBounds.Left, e.Y + _overlayBounds.Top);
				SelectedRectangle = GetRectangle(_startPoint, _endPoint);
				DialogResult = DialogResult.OK;
				Close();
			}
			base.OnMouseUp(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (_dragging)
			{
				Rectangle rect = GetRectangle(
					new Point(_startPoint.X - _overlayBounds.Left, _startPoint.Y - _overlayBounds.Top),
					new Point(_endPoint.X - _overlayBounds.Left, _endPoint.Y - _overlayBounds.Top)
				);
				using (Pen pen = new Pen(Color.Red, 2))
				{
					e.Graphics.DrawRectangle(pen, rect);
				}
				using (Brush brush = new SolidBrush(Color.FromArgb(50, Color.Blue)))
				{
					e.Graphics.FillRectangle(brush, rect);
				}
			}
		}

		private Rectangle GetRectangle(Point _p1, Point _p2)
		{
			int x = Math.Min(_p1.X, _p2.X);
			int y = Math.Min(_p1.Y, _p2.Y);
			int w = Math.Abs(_p1.X - _p2.X);
			int h = Math.Abs(_p1.Y - _p2.Y);
			return new Rectangle(x, y, w, h);
		}
	}
}
