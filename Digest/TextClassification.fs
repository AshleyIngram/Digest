namespace Digest.TextAnalysis.Classification

open Digest.TextAnalysis.Helpers

type Classification = 
    | Good
    | Bad

module NaiveBayesClassifier =
    let classify(goodCount: int, badCount: int, goodPercentage: float) =
        let totalOccurrences = goodCount + badCount
        let badProbability = float badCount / float totalOccurrences
        let goodProbability =  1.0 - badProbability
        let badPercentage = 1.0 - goodPercentage
        let badCombo = badProbability * badPercentage
        let goodCombo = goodProbability * goodPercentage
        goodCombo / (badCombo + goodCombo)