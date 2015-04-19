namespace Digest

open System
open System.Net
open System.Text.RegularExpressions
open FSharp.Data
open Microsoft.FSharp.Control.WebExtensions

type ArticleType = { Text: string }
type RankedArticleType = { Article: ArticleType; Rank: double }

module Article =
    
    let private downloadFromUri uri = 
            let webClient = new WebClient()
            webClient.AsyncDownloadString(uri)
    
    let Create uri =
        async {
            let! content = downloadFromUri uri
            return { Text = content }
        }

    let RankArticle article = 
        { Article = article; Rank = 0.0 }

    let ExtractLinks body = 
        let html = HtmlDocument.Parse(body.Text)
        let links = html.Descendants["a"] |> Seq.choose(fun e -> e.TryGetAttribute("href")) |> Seq.map(fun l -> l.Value())
        links |> Seq.map(fun l -> new Uri(l))