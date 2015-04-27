module Digest.Pipeline

open System
open System.Diagnostics
open System.Net

type IActor<'a> = 
    abstract member Cancel: unit -> unit
    abstract member Post: 'a -> unit

type Cancellable<'a> =
    | Value of 'a
    | Cancelled

let handleFailedWebRequest (httpWebResponse: HttpWebResponse) =
    let responseUri = httpWebResponse.ResponseUri.ToString()
    let status = httpWebResponse.StatusCode.ToString()
    let traceMessage = sprintf "WebException occurred: %s %s" responseUri status
    Trace.TraceError(traceMessage)

type FetchArticlesActor() = 
    
    let mutable children: IActor<'ArticleType> list = []

    let rec mailbox = MailboxProcessor.Start(fun inbox ->
        let rec loop() = 
            async {
                let! message = inbox.Receive()

                match message with
                | Cancelled -> return 0
                | Value uri ->
                    try 
                        let! article = Article.Create uri
                        let links = Article.ExtractLinks article
                        children |> Seq.iter (fun c -> c.Post(article))
                        links |> Seq.iter (fun l -> mailbox.Post(Value(l)))
                        return! loop()
                    with
                        | :? UriFormatException as e ->
                            Trace.TraceError(sprintf "Invalid URI: %s" e.Message)
                            return! loop()
                        | :? WebException as e when (e.Response :? HttpWebResponse) ->
                            let httpWebResponse = e.Response :?> HttpWebResponse
                            handleFailedWebRequest httpWebResponse
                            return! loop()
            }

        loop() |> Async.Ignore
    )
    
    member this.Cancel() = (this :> IActor<_>).Cancel()
    member this.Post(v) = (this :> IActor<_>).Post(v)
    member this.AddChild(c) = (children <- (c :: children))

    interface IActor<Uri> with
        member this.Cancel() = mailbox.Post(Cancelled)
        member this.Post(uri) = mailbox.Post(Value uri)

type DedupeActor() = 
    let mutable children: IActor<Uri> list = []
    let mutable messages = new Set<string>(Seq.empty)

    let rec mailbox = MailboxProcessor.Start(fun inbox ->
        let rec loop() = 
            async {
                let! message = inbox.Receive()
                
                match message with
                    | Cancelled -> return 0
                    | Value uri ->
                        let value = uri.ToString()
                        if messages.Contains value then 
                            return! loop()
                        else
                            messages <- messages.Add value
                            children |> Seq.iter (fun c -> c.Post(uri))
                            return! loop()
            }

        loop() |> Async.Ignore
    )

    member this.Cancel() = (this :> IActor<_>).Cancel()
    member this.Post(v) = (this :> IActor<_>).Post(v)
    member this.AddChild(c) = (children <- (c :: children))

    interface IActor<Uri> with
        member this.Cancel() = mailbox.Post(Cancelled)
        member this.Post(uri) = mailbox.Post(Value uri)
            