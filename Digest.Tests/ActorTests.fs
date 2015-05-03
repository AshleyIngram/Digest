module Digest.Tests.PipelineTests

open System
open System.Threading
open Digest
open Digest.Actor
open Digest.Pipeline
open FSharp.Control
open FsUnit.Xunit
open Xunit

type TestActor<'a>() = 
    let mutable recievedMessages = []

    member this.GetMessageCount() =
        recievedMessages |> Seq.length

    interface IRecievingActor<'a> with
        member this.Cancel() = ()
        member this.Post(m) = recievedMessages <- m :: recievedMessages
        

[<Fact>]
let ``The FetchArticles mailbox gets messages over time``() =
    let rootUri = new Uri("http://google.com")
    let actor = Pipeline.FetchArticlesActor
    let assertionActor = new TestActor<ArticleType>()
    actor.AddChild(assertionActor)
    actor.Post(rootUri)
    Thread.Sleep(TimeSpan.FromSeconds(2.0))
    actor.Cancel()
    assertionActor.GetMessageCount() |> should not' (equal 0)

[<Fact>]
let ``Given 2 identical URIs, the Dedupe actor will only allow 1 to be processed``() =
    let uri = new Uri("http://google.com")
    let actor = Pipeline.DedupeActor
    let assertionActor = new TestActor<Uri>()
    actor.AddChild(assertionActor)
    actor.Post(uri)
    actor.Post(uri)
    Thread.Sleep(TimeSpan.FromSeconds(1.0))
    actor.Cancel()
    assertionActor.GetMessageCount() |> should equal 1

