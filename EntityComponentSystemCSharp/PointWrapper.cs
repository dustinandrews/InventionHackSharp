namespace EntityComponentSystemCSharp.Components
{
	/// <summary>
	/// RogueSharp implements Point as a struct, which is in effect sealed.
	/// Wrapper allows various components to be points with different names under the hood.
	/// </summary>
	public class PointWrapper
	{
		RogueSharp.Point point = new RogueSharp.Point();

		public int X  { get => point.X; set => point.X = value;}
		public int Y  { get => point.Y; set => point.Y = value;}

		public PointWrapper()
		{

		}

		public PointWrapper(int value)
		{
			point = new RogueSharp.Point(value);
		}

		public PointWrapper(int x, int y)
		{
			point = new RogueSharp.Point(x,y);
		}

		PointWrapper(RogueSharp.Point point)
		{
			this.point = point;
		}

		public static PointWrapper Add(PointWrapper value1, PointWrapper value2)
		{
			var rpoint = RogueSharp.Point.Add(value1.point, value2.point);
			return new PointWrapper(rpoint);
		}

		public static float Distance(PointWrapper value1, PointWrapper value2)
		{
			return RogueSharp.Point.Distance(value1.point, value2.point);
		}

		public static PointWrapper Divide(PointWrapper value1, PointWrapper value2)
		{
			var rpoint = RogueSharp.Point.Divide(value1.point, value2.point);
			return new PointWrapper(rpoint);
		}

		public static PointWrapper Multiply(PointWrapper value1, PointWrapper value2)
		{
			var rpoint = RogueSharp.Point.Multiply(value1.point, value2.point);
			return new PointWrapper(rpoint);
		}

		public static PointWrapper Negate(PointWrapper value1)
		{
			var rpoint = RogueSharp.Point.Negate(value1.point);
			return new PointWrapper(rpoint);
		}

		public static PointWrapper Subtract(PointWrapper value1, PointWrapper value2)
		{
			var rpoint = RogueSharp.Point.Subtract(value1.point, value2.point);
			return new PointWrapper(rpoint);
		}

		public override bool Equals(object obj)
		{
			var wrapper = obj as PointWrapper;
			return wrapper != null && wrapper.point != null && point.Equals(obj);
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return point.GetHashCode();
		}

		public override string ToString()
		{
			return point.ToString();
		}

		public static PointWrapper operator +(PointWrapper value1, PointWrapper value2)
		{
			var wrapper = new PointWrapper((value1.point + value2.point));
			return wrapper;
		}

		public static PointWrapper operator -(PointWrapper value1, PointWrapper value2)
		{
			var wrapper = new PointWrapper((value1.point - value2.point));
			return wrapper;
		}

		public static PointWrapper operator *(PointWrapper value1, PointWrapper value2)
		{
			var wrapper = new PointWrapper((value1.point * value2.point));
			return wrapper;
		}

		public static PointWrapper operator /(PointWrapper source, PointWrapper divisor)
		{
			var wrapper = new PointWrapper((source.point / divisor.point));
			return wrapper;
		}
		public static bool operator ==(PointWrapper a, PointWrapper b)
		{
			return a?.point == b?.point;
		}

		public static bool operator !=(PointWrapper a, PointWrapper b)
		{
			return a?.point != b?.point;
		}


	}
}