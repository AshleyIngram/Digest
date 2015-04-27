module Digest.Tests.ArticleSourceTests

open System
open Digest
open FSharp.Control
open FsUnit.Xunit
open Xunit

[<Fact>]
let ``Getting Articles from a subreddit frontpage gives 25 URIs``() =
    let programming = new ArticleSource.RedditArticleSource("programming")
    let articles = programming.GetArticles() |> Async.RunSynchronously
    articles |> Seq.length |> should equal 25