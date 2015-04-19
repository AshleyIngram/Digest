module Digest.Tests.ArticleTests

open Digest
open FsUnit.Xunit
open Xunit

[<Fact>]
let ``Article.GetLinks gets URI of all links in a document``() =
    let html = "<a href=\"http://google.com\">Google</a><a href=\"http://bing.com\">Bing</a>"
    let article = { Text = html }
    let links = Article.ExtractLinks(article) |> Seq.map(fun l -> l.AbsoluteUri)
    links |> Seq.length |> should equal 2
    links |> should contain "http://google.com/"
    links |> should contain "http://bing.com/"

[<Fact>]
let ``Article.GetLinks does not return a URI in the body of a document, just that in a href attribute``() =
    let html = "<a href=\"http://google.com\">Google</a>. http://bing.com"
    let article = { Text = html }
    let links = Article.ExtractLinks(article) |> Seq.map(fun l -> l.AbsoluteUri)
    links |> Seq.length |> should equal 1
    links |> should contain "http://google.com/"