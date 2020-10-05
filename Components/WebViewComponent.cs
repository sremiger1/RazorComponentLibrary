using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Components
{
	[ViewComponent]
	public abstract class WebViewComponent : ViewComponent
	{
		public IViewComponentResult NoContent() => Content(string.Empty);

		public new IViewComponentResult View()
		{
			return View(viewName: null);
		}

		public new IViewComponentResult View(string viewName)
		{
			return View(viewName, ViewData.Model);
		}

		public new IViewComponentResult View<TModel>(TModel model)
		{
			return View(viewName: null, model: model);
		}

		public new IViewComponentResult View<TModel>(string viewName, TModel model)
		{
			return new WebViewComponentResult
			{
				ViewEngine = ViewEngine,
				ViewName = viewName,
				ViewData = new ViewDataDictionary<TModel>(ViewData, model)
			};
		}

		protected T TryGetService<T>(Type type) => (T)HttpContext.RequestServices.GetService(type);
	}
}
