using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Components
{
	public class WebViewComponentResult : IViewComponentResult
	{
		private const string DefaultViewName = "Default";

		// {0} is the component name, {1} is the view name.
		private const string ViewPathFormat = "{0}/{1}";

		public ViewDataDictionary ViewData { get; set; }
		public IViewEngine ViewEngine { get; set; }
		public string ViewName { get; set; }

		public void Execute(ViewComponentContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			var task = ExecuteAsync(context);
			task.GetAwaiter().GetResult();
		}

		public async Task ExecuteAsync(ViewComponentContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			var viewEngine = ViewEngine ?? ResolveViewEngine(context);
			var viewContext = context.ViewContext;
			var isNullOrEmptyViewName = string.IsNullOrEmpty(ViewName) || ViewName == DefaultViewName;

			ViewEngineResult result = null;
			IEnumerable<string> originalLocations = null;
			if (!isNullOrEmptyViewName)
			{
				// If view name was passed in is already a path, the view engine will handle this.
				result = viewEngine.GetView(viewContext.ExecutingFilePath, ViewName, isMainPage: false);
				originalLocations = result.SearchedLocations;
			}

			if (result == null || !result.Success)
			{
				// IMPORTANT: This strips away the default naming convention that forces all View Components
				// to be placed in ~/Components/ComponentName/View.cshtml and instead allows for
				// significantly more customization as well as sub-components nested inside other
				// components' folders
				var qualifiedViewName = isNullOrEmptyViewName ? string.Format(
					CultureInfo.InvariantCulture,
					ViewPathFormat,
					context.ViewComponentDescriptor.ShortName,
					DefaultViewName)
					: ViewName;
				result = viewEngine.FindView(viewContext, qualifiedViewName, isMainPage: false);
			}

            var view = result.EnsureSuccessful(originalLocations).View;
            using (view as IDisposable)
            {
                var childViewContext = new ViewContext(
                    viewContext,
                    view,
                    ViewData ?? context.ViewData,
                    context.Writer);
				// IMPORTANT: This Try-Catch Encapsulates our modules ensuring an error in one module does not
				//   incur a 500 on the page and instead simply doesn't render anything
                try
                {
                    await view.RenderAsync(childViewContext);
                }
                catch (Exception e)
				{
					var logger = ResolveLogger(context);
					if (logger != null)
					{
						logger.LogError($"Failed to render module '{context.ViewComponentDescriptor.ShortName}'", e);
						logger.LogDebug(e.Message);
					}
				}
			}
		}

		private static ILogger ResolveLogger(ViewComponentContext context)
		{
			return context.ViewContext.HttpContext.RequestServices.GetRequiredService<ILogger<WebViewComponentResult>>();
		}

		private static IViewEngine ResolveViewEngine(ViewComponentContext context)
		{
			return context.ViewContext.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
		}
	}
}
