using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using static Common.Infrastructure.Controller.HelperMethods;

namespace Common.Infrastructure.Controller
{
    public class GenericControllerRouteConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.IsGenericType)
            {
                var genericType = controller.ControllerType.GenericTypeArguments[0];
                var feature = GetFeatureName(genericType);

                var selectorModel = new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel(new RouteAttribute($"features/{feature}")),
                };

                selectorModel.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { "GET" }));

                controller.Selectors.Add(selectorModel);
            }
        }
    }
}
