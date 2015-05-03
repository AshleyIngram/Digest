module Digest.Pipeline

open System
open System.Diagnostics
open System.Net
open Digest.Actor
open Digest.TextAnalysis.Stemming

let handleFailedWebRequest (httpWebResponse: HttpWebResponse) =
    let responseUri = httpWebResponse.ResponseUri.ToString()
    let status = httpWebResponse.StatusCode.ToString()
    let traceMessage = sprintf "WebException occurred: %s %s" responseUri status
    Trace.TraceError(traceMessage)

let fetchArticles(uri, state) =
    async {
        try 
            let! article = Article.Create uri
            let links = Article.ExtractLinks article
            state.children |> Seq.iter (fun c -> c.Post(article))
            links |> Seq.iter (fun l -> state.self.Post(l))
        with
            | :? UriFormatException as e ->
                Trace.TraceError(sprintf "Invalid URI: %s" e.Message)
                
            | :? WebException as e when (e.Response :? HttpWebResponse) ->
                let httpWebResponse = e.Response :?> HttpWebResponse
                handleFailedWebRequest httpWebResponse

        return state
    }

let deduplicateUris(uri, state: State<Uri, Uri, Set<string>>) =
    let uriString = uri.ToString()
    async {
        if (state.value.Contains(uriString)) then
            return state
        else 
            let newState = { self = state.self; children = state.children; value = state.value.Add(uriString) }
            state.children |> Seq.iter (fun c -> c.Post(uri))
            return newState
    }

let stemArticle(article, state) =
    async {
        let stems = article.Text.Split(' ') |> Seq.map (fun w -> stem w)
        let result = (article, stems)
        state.children |> Seq.iter (fun c -> c.Post(result))
        return state
    } 

let FetchArticlesActor = new Actor<Uri, ArticleType, Object>(fetchArticles, None)
let DedupeActor = new Actor<Uri, Uri, Set<string>>(deduplicateUris, Set.empty)
let StemmingActor = new Actor<ArticleType, (ArticleType * seq<string>), Object>(stemArticle, None) 