namespace UmatoMusume.Models
{
	public class RectConfig
	{
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public string RectName { get; set; } = string.Empty;
		public int X { get; set; }
		public int Y { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		public RectConfig() { }

		public RectConfig(string _rectName, int _x, int _y, int _width, int _height)
		{
			RectName = _rectName;
			X = _x;
			Y = _y;
			Width = _width;
			Height = _height;
		}

		public Rectangle ToRectangle()
		{
			return new Rectangle(X, Y, Width, Height);
		}
	}
}
