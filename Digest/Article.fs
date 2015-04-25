namespace Digest

open System
open System.Text.RegularExpressions
open FSharp.Data

type ArticleType = { Text: string }
type RankedArticleType = { Article: ArticleType; Rank: double }

module Article =
   
    let Create uri =
        async {
            let! content = Helpers.downloadFromUri uri
            return { Text = content }
        }

    let RankArticle article = 
        { Article = article; Rank = 0.0 }

    let ExtractLinks body = 
        let html = HtmlDocument.Parse(body.Text)
        let links = html.Descendants["a"] |> Seq.choose(fun e -> e.TryGetAttribute("href")) |> Seq.map(fun l -> l.Value())
        links |> Seq.map(fun l -> new Uri(l))