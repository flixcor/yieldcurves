using System;

namespace CurveRecipes.Domain
{
    public class OrderedTransformation
    {
        public OrderedTransformation(Order order, ITransformation transformation)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
            Transformation = transformation ?? throw new ArgumentNullException(nameof(transformation));
        }

        public Order Order { get; private set; }
        public ITransformation Transformation { get; }
    }
}
