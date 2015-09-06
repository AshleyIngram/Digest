module Digest.Article

open System
open System.Text.RegularExpressions
open Digest.Types
open FSharp.Data

let Create uri =
    async {
        let! content = Helpers.DownloadFromUri uri
        return { Text = content; Uri = uri; }
    }

let ClassifyArticle article = 
    0.0

let ExtractLinks body = 
    let html = HtmlDocument.Parse(body.Text)
    let links = html.Descendants["a"] |> Seq.choose(fun e -> e.TryGetAttribute("href")) |> Seq.map(fun l -> l.Value())
    links |> Seq.filter (fun l -> Uri.IsWellFormedUriString(l, UriKind.Absolute)) |> Seq.map(fun l -> new Uri(l))