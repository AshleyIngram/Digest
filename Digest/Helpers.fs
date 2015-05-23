module Digest.Helpers

open System.Net
open FSharp.Data

let downloadFromUri uri = 
        let webClient = new WebClient()
        webClient.AsyncDownloadString(uri)

let failedWebRequestString (httpWebResponse: HttpWebResponse) =
    let responseUri = httpWebResponse.ResponseUri.ToString()
    let status = httpWebResponse.StatusCode.ToString()
    sprintf "WebException occurred: %s %s" responseUri status