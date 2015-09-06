namespace Digest

open System
open Digest.Types
open Digest.PersistentCollections
open Digest.TextAnalysis.Helpers

type State() =
    let queue = new PersistentQueue<Uri>("ArticlesToProcess")
    let classifications = new PersistentDictionary<Uri, double>("ArticleRanks")
    let wordranks = new PersistentDictionary<string, (int * int)>("WordRanks")
    let threshold = 0.5

    member this.QueueForProcessing uri = queue.Enqueue uri

    member this.GetNextUri() = queue.Dequeue()

    member this.HasProcessedArticle uri = classifications.Contains uri

    member this.UpdateWord word isGood = 
        let (good, bad) = match wordranks.GetOrDefault(word) with
                            | Some x -> x
                            | None -> (0, 0)

        let newState = if isGood then (good + 1, bad) else (good, bad + 1)
        wordranks.AddOrUpdate(word, newState)

    member this.SetRanking article rank = 
        classifications.AddOrUpdate(article.Uri, rank)
        let isGood = rank >= threshold
        article.Text |> stemText |> Seq.iter (fun w -> this.UpdateWord w isGood)

    member this.GetWordStats word = wordranks.Get word

    member this.GetArticleRank uri = classifications.Get uri

    member this.GetArticleCount() = classifications.getCollection() |> Seq.length

    member this.GetTopRecommended count = classifications.getCollection() |> Seq.sortBy (fun (KeyValue(k, v)) -> v) |> Seq.truncate count

    member this.Delete() = 
        queue.delete()
        classifications.delete()
        wordranks.delete()