module Digest.Tests.PipelineTests

open System
open System.Threading
open Digest
open Digest.Pipeline
open FSharp.Control
open FsUnit.Xunit
open Xunit

type TestActor() = 
    let mutable recievedMessages = []

    member this.GetMessageCount() =
        recievedMessages |> Seq.length

    interface IActor<ArticleType> with
        member this.Cancel() = ()
        member this.Post(m) = recievedMessages <- m :: recievedMessages
        

[<Fact>]
let ``The FetchArticles mailbox gets messages over time``() =
    let rootUri = new Uri("http://google.com")
    let actor = new Pipeline.FetchArticles()
    let assertionActor = new TestActor()
    actor.AddChild(assertionActor)
    actor.Post(rootUri)
    Thread.Sleep(TimeSpan.FromSeconds(10.0))
    actor.Cancel()
    assertionActor.GetMessageCount() |> should not' (equal 0)
