﻿module Digest.TextAnalysis.Tests.StemmingTests

open Digest.TextAnalysis.Stemming
open FsUnit.Xunit
open Xunit

let inline (=>) (x: string) (y: string) = x |> stemWord |> should equal y
let inline (==>) (x: string) (y: string[]) = x |> stemText |> Seq.toArray |> should equal y

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