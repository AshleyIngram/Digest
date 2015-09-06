module Digest.TextAnalysis.Helpers.Tests

open Digest.TextAnalysis.Helpers
open Digest.Types
open FsUnit.Xunit
open Xunit

let inline (=>) (x: string) (y: string) = x |> StemWord |> should equal y
let inline (==>) (x: string) (y: string[]) = x |> StemText |> Seq.toArray |> should equal y

[<Fact>]
let ``Words should be stemmed to a common root``() =
    "account" => "account"
    "accountabilities" => "account"
    "accountability" => "account"
    "accountable" => "account"
    "accountant" => "account"
    "accounted" => "account"
    "accounting" => "account"

[<Fact>]
let ``Words should be 'normalized' by lower casing and removing punctuation``() =
    "Testing" => "test"
    "Fishing" => "fish"
    "Fishing." => "fish"

[<Fact>]
let ``Text is stemmed as a series of words``() =
    "Testing code" ==> [| "test";  "code" |]

[<Fact>]
let ``Stopwords are removed when stemming a sentence``() =
    "When I went to the shop, I went to go shopping" ==> [| "went"; "shop"; "went"; "go"; "shop" |]

[<Fact>]
let ``bagOfWords returns the frequency of stemmed words in a sentence``() =
    let expectedValue = [| { Word = "went"; Frequency = 2 }; { Word = "shop"; Frequency = 2 }; { Word = "go"; Frequency = 1 } |]
    "When I went to the shop, I went to go shopping" |> BagOfWords |> Seq.toArray |> should equal expectedValue