module Digest.Console.Program

open System
open System.Diagnostics
open System.Net
open Digest
open Digest.Article
open Digest.ArticleSource
open Digest.Helpers
open Digest.PersistentCollections

let rankingDictionary = new PersistentDictionary<Uri, double>("ArticleRanks")

let enqueueUris(queue: PersistentQueue<Uri>, uris) = 
    uris |> Seq.iter (fun q -> queue.Enqueue(q))

let getArticle(uri) =
    async {
        try
            let! a = Article.Create uri
            return Some(a)
        with
            | :? UriFormatException as e ->
                Trace.TraceError(sprintf "Invalid URI: %s" e.Message)
                return None
            | :? WebException as e when (e.Response :? HttpWebResponse) ->
                let httpWebResponse = e.Response :?> HttpWebResponse
                Trace.TraceWarning(Digest.Helpers.failedWebRequestString httpWebResponse)
                return None
            | :? WebException as e ->
                Trace.TraceWarning(e.ToString())
                return None
    }

let processUri(uri) =
    async {
        if (rankingDictionary.Contains(uri)) then
            return Seq.empty
        else
            let! a = getArticle(uri) 
            match a with
                | None -> return Seq.empty
                | Some(article) ->
                    let rank = Article.RankArticle(article)
                    rankingDictionary.Set(uri, rank)
                    return Article.ExtractLinks(article)
    }

let rec processQueue(item, queue: PersistentQueue<Uri>) =
    async {
        match item with
            | Some(x) -> 
                Trace.TraceInformation("Processing " + x.ToString())
                let! uris = processUri(x)
                enqueueUris(queue, uris)
                return! processQueue(queue.Dequeue(), queue)
            | None -> return ()
    }

let runRankingProgram = async {
    let uriQueue = new PersistentQueue<Uri>("ArticlesToProcess");
    let reddit = new RedditArticleSource("programming")
    let! uris = reddit.GetArticles()
    enqueueUris(uriQueue, uris)

    return! processQueue(uriQueue.Dequeue(), uriQueue)
}

let showStats() = 
    let dict = (new PersistentDictionary<Uri, double>("ArticleRanks")).getCollection()
    let count = dict.Count
    let top10 = dict |> Seq.sortBy (fun (KeyValue(k, v)) -> v) |> Seq.truncate 10
    printfn "Processed %d articles" count
    printfn "The top 10 articles are..."
    top10 |> Seq.map (fun(KeyValue(k, v)) -> k.ToString()) |> Seq.iter(fun (l) -> printfn "%s" l)

[<EntryPoint>]
let main argv = 
    Trace.Listeners.Add(new ConsoleTraceListener()) |> ignore

    printfn "Please enter 'Run' or 'Show'"
    let userChoice = Console.ReadLine()

    match userChoice.ToLowerInvariant() with 
        | "run" -> runRankingProgram |> Async.RunSynchronously
        | "show" -> showStats()
        | _ -> printfn "Unrecognized Input"

    Console.ReadLine() |> ignore
    0