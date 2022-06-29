

using System;

namespace APIMAuthorizationsDemo.Function
{
    public class GithubDiscussionComment
    {
        public GithubAuthor Author { get; set; }
        public string Body { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Id { get; set; }
        public string Url { get; set; }
        
    }

  public class GithubAuthor
  {
    public string Login { get; set; }
  }
}