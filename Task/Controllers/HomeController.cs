
using System.Web.Mvc;
using Task.Models;
namespace Task.Controllers
{

	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View("Index", new ResponseStructure(new StatusStructure()));
		}


		[HttpPost]
		public ActionResult Index(string url)
		{
			StatusStructure statusStructure = new StatusStructure();
			string fixedGithubUrl = "https://github.com/";
			int fixedGithubUrlLength = fixedGithubUrl.Length;
			if (!url.Contains(fixedGithubUrl))
			{
				statusStructure.m_statusCode = 201;
				statusStructure.m_statusMessage = "Please enter a valid github repo";
				return View("Index", new ResponseStructure(statusStructure));
			}
			
			string retrieveApiUrl = url.Substring(fixedGithubUrlLength);
			if (!retrieveApiUrl.EndsWith("/"))
			{
				retrieveApiUrl += "/";
			}
			return View("Index", new ResponseStructure(retrieveApiUrl, statusStructure));
		}

	}
}
