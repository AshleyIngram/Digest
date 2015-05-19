module Digest.TextAnalysis.Stemming

open System
open Iveonik.Stemmers

let private stemmer = new EnglishStemmer() 

let private removePunctuation (s: string) = 
    new string(s.ToCharArray() |> Seq.filter(fun c -> not(Char.IsPunctuation(c))) |> Array.ofSeq)

let stem (word: string) = 
    let normalizedWord = word.ToLowerInvariant() |> removePunctuation
    stemmer.Stem(normalizedWord)
