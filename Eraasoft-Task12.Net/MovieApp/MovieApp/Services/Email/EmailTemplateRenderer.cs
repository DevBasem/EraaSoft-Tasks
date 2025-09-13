using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text;

namespace MovieApp.Services.Email
{
    public interface IEmailTemplateRenderer
    {
        Task<string> RenderEmailTemplateAsync<TModel>(string viewName, TModel model);
    }

    public class EmailTemplateRenderer : IEmailTemplateRenderer
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailTemplateRenderer(
            IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> RenderEmailTemplateAsync<TModel>(string viewName, TModel model)
        {
            var httpContext = _httpContextAccessor.HttpContext ?? new DefaultHttpContext { RequestServices = _serviceProvider };
            
            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor());

            using (var sw = new StringWriter())
            {
                var viewPath = $"~/Views/EmailTemplates/{viewName}.cshtml";
                
                // Find the view
                var viewEngineResult = _razorViewEngine.FindView(actionContext, viewPath, false);
                if (!viewEngineResult.Success)
                {
                    // Try to get the view directly if finding fails
                    viewEngineResult = _razorViewEngine.GetView(null, viewPath, false);
                    
                    if (!viewEngineResult.Success)
                    {
                        throw new InvalidOperationException($"Could not find email template: {viewName}");
                    }
                }

                var viewDictionary = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary())
                {
                    Model = model
                };
                
                // Add layout path to ViewData to ensure it's found correctly
                viewDictionary["_ViewStart"] = null; // Skip _ViewStart processing
                viewDictionary["Layout"] = "~/Views/EmailTemplates/_EmailLayout.cshtml";

                var tempData = new TempDataDictionary(
                    httpContext,
                    _tempDataProvider);

                var viewContext = new ViewContext(
                    actionContext,
                    viewEngineResult.View,
                    viewDictionary,
                    tempData,
                    sw,
                    new HtmlHelperOptions());

                await viewEngineResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}