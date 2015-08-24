module Digest.Types

open System

type ArticleType = { Text: string; Uri: Uri }

type BagOfWords = { Word: string; Frequency: int }

type Classification = 
    | Good
    | Bad