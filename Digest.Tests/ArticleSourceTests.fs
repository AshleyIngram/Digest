module Digest.Tests.ArticleSourceTests

open System
open Digest
open FSharp.Control
open FsUnit.Xunit
open Xunit

[<Fact>]
let ``Getting Articles from a subreddit frontpage gives 25 articles``() =
    let programming = new ArticleSource.RedditArticleSource("programming")
    let articles = programming.GetArticles() |> AsyncSeq.toBlockingSeq
    articles |> Seq.length |> should equal 25