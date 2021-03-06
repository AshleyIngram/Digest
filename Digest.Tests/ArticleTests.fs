﻿module Digest.Tests.ArticleTests

open System
open Digest
open Digest.Types
open FsUnit.Xunit
open Xunit

[<Fact>]
let ``Article.GetLinks gets URI of all links in a document``() =
    let html = "<a href=\"http://google.com\">Google</a><a href=\"http://bing.com\">Bing</a>"
    let article = { Text = html; Uri = new Uri("http://example.com") }
    let links = Article.ExtractLinks(article) |> Seq.map(fun l -> l.AbsoluteUri)
    links |> Seq.length |> should equal 2
    links |> should contain "http://google.com/"
    links |> should contain "http://bing.com/"

[<Fact>]
let ``Article.GetLinks does not return a URI in the body of a document, just that in a href attribute``() =
    let html = "<a href=\"http://google.com\">Google</a>. http://bing.com"
    let article = { Text = html; Uri = new Uri("http://example.com") }
    let links = Article.ExtractLinks(article) |> Seq.map(fun l -> l.AbsoluteUri)
    links |> Seq.length |> should equal 1
    links |> should contain "http://google.com/"

[<Fact>]
let ``Creating an Article from a URI gets the content of the article by downloading the webpage``() =
    let uri = new Uri("http://example.com")
    let article = Article.Create(uri) |> Async.RunSynchronously
    article.Text.Contains "<h1>Example Domain</h1>" |> should be True