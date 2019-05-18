using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using Task.Models;

namespace Task.Helpers
{
	public abstract class HttpResponse
	{
		/* Returns the HttpResponse 
		*  This is needed for getting the response headers so as to make out 
		*  whether pagination is needed or not
		*/
		public static HttpWebResponse Response(string url, ref StatusStructure statusStructure)
		{
			try
			{
				if (url.EndsWith("/"))
				{
					url = url.TrimEnd('/');
				}
				var request = (HttpWebRequest)WebRequest.Create(url);
				request.UserAgent = "request";
				return (HttpWebResponse)request.GetResponse();
			}
			catch (WebException ex)
			{
				var exception = ex.Response as HttpWebResponse;
				statusStructure.m_statusCode = (int)exception.StatusCode;
				statusStructure.m_statusMessage = ex.Message;
				return exception;
			}
		}

		//Function to fetch the actual response to see if the Object is returned has the values
		public static JObject GetReponseAsync(string url, ref StatusStructure statusStructure)
		{
			try
			{
				HttpWebResponse response = Response(url, ref statusStructure);
				var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
				JObject responseObject =  JObject.Parse(responseString);
				statusStructure.m_statusCode = 200;
				statusStructure.m_statusMessage = "OK";
				return responseObject;
			}
			catch (Exception ex)
			{
				statusStructure.m_statusCode = 400;
				statusStructure.m_statusMessage = ex.Message;
				return new JObject();
			}

		}

		//To find the array response when queried as the issues URL
		//The function is seperated as the functionality can be further used by the project over time
		//Owing to the fact we need one of the function to just return the array response and the other to return
		//The simple json response
		public static JArray GetReponseAsyncArray(string url, ref StatusStructure statusStructure)
		{
			try
			{
				HttpWebResponse response = Response(url, ref statusStructure);
				var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
				JArray responseArray =  JArray.Parse(responseString);
				statusStructure.m_statusCode = 200;
				statusStructure.m_statusMessage = "OK";
				return responseArray;
			}
			catch (Exception ex)
			{
				statusStructure.m_statusCode = 400;
				statusStructure.m_statusMessage = ex.Message;
				return new JArray();
			}
		}

		//The function ot return the next url from the response headers as the API 
		//explains
		public static string GetLink(HttpWebResponse response)
		{
			string[] links = response.Headers["Link"].ToString().Split(',');
			foreach (string link in links)
			{
				if (link.Contains("rel=\"next\""))
				{
					string nextLink = link.Split(';')[0];
					nextLink = nextLink.Trim();
					nextLink = nextLink.TrimStart('<');
					nextLink = nextLink.TrimEnd('>');
					return nextLink;
				}
			}
			return null;
		}

		//Function to find whether the Url to the next page really exists
		public static bool ContainsLinkHeaderRelNext(HttpWebResponse response)
		{
			WebHeaderCollection headers = response.Headers;
			if (headers["Link"] != null)
			{
				if (headers["Link"].Contains("rel=\"next\""))
				{
					return true;
				}
			}
			return false;
		}
	}
}