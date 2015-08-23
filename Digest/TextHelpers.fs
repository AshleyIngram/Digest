module Digest.TextAnalysis.Helpers

open System
open System.IO
open Digest.TextAnalysis.Stopwords
open FSharp.Core
open Iveonik.Stemmers

let private stemmer = new EnglishStemmer() 

let private removePunctuation (s: string) = 
    new string(s.ToCharArray() |> Array.filter(fun c -> not(Char.IsPunctuation(c))))

let private normalizeString (s: string) =
    s.ToLowerInvariant() |> removePunctuation

let stemWord word = 
    word |> normalizeString |> stemmer.Stem

let stemText (text: string) =
    let stopwords = Stopwords.stopwords |> Array.map normalizeString
    let notStopword w = (Array.exists (fun e -> e = w) stopwords) |> not
    text.Split(' ') |> Seq.map stemWord |> Seq.filter notStopword

type BagOfWords = { word: string; frequency: int }

let bagOfWords (text: string) =
    stemText text |> Seq.countBy id |> Seq.map (fun (w, f) -> { word = w; frequency = f })