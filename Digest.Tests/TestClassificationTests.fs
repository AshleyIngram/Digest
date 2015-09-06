module Digest.Tests.TextClassificationTests

open System
open Digest.TextAnalysis.Classification
open FsUnit.Xunit
open Xunit

let truncate places x = 
    let exp = 10.0 ** float places
    truncate (x * exp) / exp

[<Fact>]
let ``Naive Bayes algorithm returns correct probabilities for binary classification``() =
    NaiveBayesClassifier.Classify(10, 10, 0.5) |> should equal 0.5
    NaiveBayesClassifier.Classify(5, 360, 0.9) |> truncate 3 |> should equal 0.111