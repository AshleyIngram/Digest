module Digest.TextAnalysis.Stemming

open Iveonik.Stemmers

let private stemmer = new EnglishStemmer() 

let stem word = 
    stemmer.Stem(word)
