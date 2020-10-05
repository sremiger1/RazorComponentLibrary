using Microsoft.AspNetCore.Mvc;
using System;

namespace Components.Components.CurrentTime
{
    public class CurrentTimeViewComponent : WebViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View(new CurrentTimeModel()
			{
				CurrentDate = System.DateTime.Now,
				CurrentTime = System.DateTime.Now.ToString()
			});
		}
	}

	public class CurrentTimeModel
	{
		public string CurrentTime { get; set; }
		public DateTime CurrentDate { get; set; }
	}
}
