module Digest.State.Tests

open System
open Digest
open Digest.Types
open FsUnit.Xunit
open Xunit

[<Fact>]
let ``UpdateWords increments the 'good' count when a word is good``() =
    let state = new State()
    try
        state.UpdateWord "test" true
        state.GetWordStats "test" |> should equal (1, 0)
    finally
        state.Delete()

[<Fact>]
let ``UpdateWords increments the 'bad' count when a word is bad``() =
    let state = new State()
    try
        state.UpdateWord "test" false
        state.GetWordStats "test" |> should equal (0, 1)
    finally
        state.Delete()

[<Fact>]
let ``SetRanking above threshold stores the ranking and increments the count for all words``() =
    let state = new State()
    try
        let uri = new Uri("http://example.com")
        let article = { Text = "When I go to the shop, I go to go shopping"; Uri = uri; }
        state.SetRanking article 0.6 |> ignore
        state.GetArticleRank uri |> should equal 0.6
        state.GetWordStats "shop" |> should equal (2, 0)
    finally
        state.Delete()
        
[<Fact>]
let ``SetRanking below threshold stores the ranking and increments the count for all words``() =
    let state = new State()
    try
        let uri = new Uri("http://example.com")
        let article = { Text = "When I go to the shop, I go to go shopping"; Uri = uri; }
        state.SetRanking article 0.4 |> ignore
        state.GetArticleRank uri |> should equal 0.4
        state.GetWordStats "shop" |> should equal (0, 2)
    finally
        state.Delete()