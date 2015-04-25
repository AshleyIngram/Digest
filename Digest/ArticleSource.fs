module Digest.ArticleSource

open System
open FSharp.Control
open FSharp.Data

type IArticleSource = 
    abstract member GetArticles: unit -> AsyncSeq<ArticleType>

type RedditTypeProvider = JsonProvider<"Data/SubredditListingExample.json">

type RedditArticleSource(subreddit: string) =
    
    let downloadRedditPage = 
            let uri = sprintf "http://reddit.com/r/%s/hot.json" subreddit
            RedditTypeProvider.AsyncLoad(uri)
    
    let getSubmissionLinks (redditPage: RedditTypeProvider.Root) =
        redditPage.Data.Children |> Seq.map (fun c -> c.Data.Url)
        
    member this.GetArticles() = (this :> IArticleSource).GetArticles() 

    interface IArticleSource with
        member this.GetArticles() =
            asyncSeq {
                let! redditPage = downloadRedditPage
                let articles = redditPage |> getSubmissionLinks |> Seq.map(fun l -> new Uri(l)) |> Seq.map Article.Create
                
                for article in articles do
                    let! a = article
                    yield a
            }