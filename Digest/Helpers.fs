module Digest.Helpers

open System.Net
open FSharp.Data

let DownloadFromUri uri = 
        let webClient = new WebClient()
        webClient.AsyncDownloadString(uri)

let FailedWebRequestString (httpWebResponse: HttpWebResponse) =
    let responseUri = httpWebResponse.ResponseUri.ToString()
    let status = httpWebResponse.StatusCode.ToString()
    sprintf "WebException occurred: %s %s" responseUri status