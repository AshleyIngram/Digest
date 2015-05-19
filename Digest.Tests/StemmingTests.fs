module Digest.TextAnalysis.Tests.StemmingTests

open Digest.TextAnalysis.Stemming
open FsUnit.Xunit
open Xunit

let inline (=>) (x: string) (y: string) = x |> stem |> should equal y

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