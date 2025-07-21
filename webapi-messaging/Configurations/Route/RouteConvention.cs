using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace webapi_messaging.Configurations.Route
{
    public class RouteConvention : IApplicationModelConvention
    {
        /// <summary>
        /// Define a route prifix for all controllers in the application.
        /// </summary>
        private readonly AttributeRouteModel _routePrefix;

        /// <summary>
        /// Create a new instance of the <see cref="RouteConvention"/> class with a specified route template provider.
        /// </summary>
        /// <param name="routeTemplateProvider"></param>
        public RouteConvention(IRouteTemplateProvider routeTemplateProvider)
        {
            _routePrefix = new AttributeRouteModel(routeTemplateProvider);
        }

        /// <summary>
        /// Apply a route prefix to all controllers in the application.
        /// </summary>
        /// <param name="application"></param>
        public void Apply(ApplicationModel application)
        { 
            foreach(var controllers in application.Controllers)
            {
                var matchedSelectors = controllers.Selectors
                    .Where(selector => selector.AttributeRouteModel != null)
                    .ToList();

                if (!matchedSelectors.Any())
                { 
                    continue;
                }

                foreach (var selector in matchedSelectors)
                {
                    selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                        _routePrefix, selector.AttributeRouteModel);
                }

                var unmatchedSelectors = controllers.Selectors
                    .Where(selector => selector.AttributeRouteModel == null)
                    .ToList();
                foreach (var selector in unmatchedSelectors)
                {
                    selector.AttributeRouteModel = _routePrefix;
                }
            }
        }
    }
}
