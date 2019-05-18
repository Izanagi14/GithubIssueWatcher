# GithubIssueWatcher
Github Open Issues

***The Application uses the Github api's for the access to the public repos, to fetch their own issues.***

####Funcitonality Description####
  1) The total number of open issues is read from the **https://api.github.com/repos/{repo_name}**
  2) The number of open issues in the last 24 hours is take from **https://api.github.com/repos/{repo_name}/issues?since={TIME}&state=open**
  3) The number of open issues in 24 hours to 7 days is done manipulating the time.
  4) The number of open issues earlier than 7 days is by subtracting the total_count with the sum of the above two.

####Tasks####  
  [x] Successfully checks whether the url entered is correct and is from github.
  [x] Fails if the Url is not correct, giving either the error saying the url is incorrect or might give an error saying can't read the json as the Url is incorrect.
  [x] On successfully run will fetch the counts and display the same.


The Url to access the page: http://atandon-001-site1.dtempurl.com/
