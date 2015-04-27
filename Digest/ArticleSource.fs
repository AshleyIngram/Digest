module Digest.ArticleSource

open System
open FSharp.Control
open FSharp.Data

type IArticleSource = 
    abstract member GetArticles: unit -> Async<seq<Uri>>

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
            async {
                let! redditPage = downloadRedditPage
                return redditPage |> getSubmissionLinks |> Seq.map(fun l -> new Uri(l))
            }