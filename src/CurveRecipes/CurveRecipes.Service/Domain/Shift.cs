namespace CurveRecipes.Domain
{
    public class Shift
    {
        public Shift(double value)
        {
            Value = value;
        }

        public double Value { get; private set; }
    }
}