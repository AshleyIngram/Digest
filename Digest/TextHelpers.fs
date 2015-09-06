module Digest.TextAnalysis.Helpers

open System
open System.IO
open Digest.Types
open Digest.TextAnalysis.Stopwords
open FSharp.Core
open Iveonik.Stemmers

let private stemmer = new EnglishStemmer() 

let private removePunctuation (s: string) = 
    new string(s.ToCharArray() |> Array.filter(fun c -> not(Char.IsPunctuation(c))))

let private normalizeString (s: string) =
    s.ToLowerInvariant() |> removePunctuation

let StemWord word = 
    word |> normalizeString |> stemmer.Stem

let StemText (text: string) =
    let stopwords = Stopwords |> Array.map normalizeString
    let notStopword w = (Array.exists (fun e -> e = w) stopwords) |> not
    text.Split(' ') |> Seq.map StemWord |> Seq.filter notStopword

let BagOfWords (text: string) =
    StemText text |> Seq.countBy id |> Seq.map (fun (w, f) -> { Word = w; Frequency = f })