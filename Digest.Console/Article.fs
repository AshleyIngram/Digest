module Digest.Console.Article

open System
open System.Diagnostics
open System.Net
open Digest
open Digest.Article

let Get uri =
    async {
        try
            let! a = Article.Create uri
            return Some(a)
        with
            | :? UriFormatException as e ->
                Trace.TraceError(sprintf "Invalid URI: %s" e.Message)
                return None
            | :? WebException as e when (e.Response :? HttpWebResponse) ->
                let httpWebResponse = e.Response :?> HttpWebResponse
                Trace.TraceWarning(Digest.Helpers.failedWebRequestString httpWebResponse)
                return None
            | :? WebException as e ->
                Trace.TraceWarning(e.ToString())
                return None
    }