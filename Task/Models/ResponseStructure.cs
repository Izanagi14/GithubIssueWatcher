
using Newtonsoft.Json.Linq;
using System;
using System.Net;


namespace Task.Models
{
	public class StatusStructure
	{
		public int m_statusCode { get; set; }
		public string m_statusMessage { get; set; }

		public StatusStructure()
		{
			m_statusCode = 200;
			m_statusMessage = "OK";
		}
	}
	public class ResponseStructure : Helpers.HttpResponse
	{

		//A status Structure for the model to be accessed as the time of displaying
		public StatusStructure m_statusStructure { get; set; }
		private string m_baseUrl = "https://api.github.com/repos/";

		public string m_repoName;
		/*
		 * Url string for the request to be sent to api to get the response
		 */
		private string m_openIssueCountUrl;

		private string m_last24HrsCountUrl;

		private string m_24To7DaysCountUrl;

		// All the open Issues that are available for the Repo
		public long m_openIssuesCountTotal { get; private set; }

		//Issues that are open in the Last 24 hrs
		public long m_opeIssuesCountIn24Hrs { get; private set; }
		//All the issues that are opened in the last 24 hrs from the Query Time and less than
		// 7 days
		public long m_openIssuesCountInLast24HrsTo7Days { get; private set; }

		//All issues older than 7 days
		public long m_openIssuesCountOlderThan7Days { get; private set; }

		public ResponseStructure(StatusStructure statusStructure)
		{
			m_statusStructure = statusStructure;
		}
		//Constructor to make different Url and Collect Data via different Functions
		public ResponseStructure(string apiUrl, StatusStructure statusStructure)
		{
			m_statusStructure = statusStructure;
			m_repoName = apiUrl;
			//Total Open Issues
			m_openIssueCountUrl = m_baseUrl + apiUrl;
			TotalOpenIssues();

			//Last 24Hrs Issues
			DateTime timeE = DateTime.Now;
			timeE = timeE.AddHours(-24);
			string time = timeE.ToString("yyyy-MM-ddTHH:mm:ss") + "Z";

			//Last 24hrs open issues count
			m_last24HrsCountUrl = m_baseUrl + apiUrl + "issues?since=" + time + "&state=open";
			m_opeIssuesCountIn24Hrs = IssueCountUrl(m_last24HrsCountUrl);

			//more than 24hrs less than 7 days
			timeE = timeE.AddDays(-7);
			time = timeE.ToString("yyyy-MM-ddTHH:mm:ss") + "Z";
			m_24To7DaysCountUrl = m_baseUrl + apiUrl + "issues?since=" + time + "&state=open";
			m_openIssuesCountInLast24HrsTo7Days = IssueCountUrl(m_24To7DaysCountUrl);


			m_openIssuesCountOlderThan7Days = m_openIssuesCountTotal - (m_opeIssuesCountIn24Hrs + m_openIssuesCountInLast24HrsTo7Days);
		}


		// Get the Overall Issues count for the Repo and assign to m_openIssuesCount
		private void TotalOpenIssues()
		{
			StatusStructure statusStructure = m_statusStructure;
			JObject jsonResponse = GetReponseAsync(m_openIssueCountUrl,ref statusStructure);
			if (jsonResponse["open_issues_count"] != null)
				m_openIssuesCountTotal = long.Parse(jsonResponse["open_issues_count"].ToString());
			else
				m_openIssuesCountTotal = 0;
		}

		//Make the Count of the Issue according to the Url specified
		private long IssueCountUrl(string issueUrl)
		{
			long issueCount = 0;
			StatusStructure statusStructure = m_statusStructure;
			HttpWebResponse response = Response(issueUrl, ref statusStructure);
			JArray jsonResponse = GetReponseAsyncArray(issueUrl, ref statusStructure);
			issueCount += jsonResponse.Count;
			while (ContainsLinkHeaderRelNext(response))
			{
				string link = GetLink(response);
				if (link != null)
				{
					response = Response(link, ref statusStructure);
					jsonResponse = GetReponseAsyncArray(link, ref statusStructure);
					issueCount += jsonResponse.Count;
				}
			}
			return issueCount;
		}
	}
}