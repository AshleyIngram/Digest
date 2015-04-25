module Digest.Helpers

open System.Net
open FSharp.Data

let downloadFromUri uri = 
        let webClient = new WebClient()
        webClient.AsyncDownloadString(uri)