using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using System.Net;

namespace Example1.Attributes
{
    public class HasFeaturesAttribute : Attribute, IAsyncActionFilter
    {
        private string[] _features;
        public HasFeaturesAttribute(params string[] features)
        {
            _features = features;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IFeatureManager? featureManager = (IFeatureManager?)context.HttpContext.RequestServices.GetService(typeof(IFeatureManager));
            if (featureManager is not null)
            {
                foreach (var feature in _features)
                {
                    if (!await featureManager.IsEnabledAsync(feature))
                        context.Result = new ObjectResult($"Feature: {feature} not enabled")
                        {
                            StatusCode = (int)HttpStatusCode.Forbidden
                        };
                }
                return;
            }
            else
            {
                context.Result = new ObjectResult("Something went wrong")
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }

}
