module Digest.PersistentCollections

open System.Collections.Generic
open System.IO
open Nessos.FsPickler

type PersistentDictionary<'k, 'v when 'k : equality>(id: string) =
    let filepath = "storage/" + id
    let pickler = FsPickler.CreateBinary()
    
    let load() = 
        let fileStream = File.OpenRead(filepath)
        pickler.Deserialize<Dictionary<'k, 'v>>(fileStream)

    let underlyingDict = if (File.Exists(filepath)) then load() else new Dictionary<'k, 'v>()

    let save() =
        let fileContents = pickler.Pickle underlyingDict
        Directory.CreateDirectory("storage") |> ignore
        let file = File.Create(filepath)
        file.Write(fileContents, 0, fileContents.Length)
        file.Close()

    member this.set(key: 'k, value: 'v) =
        underlyingDict.Add(key, value) |> ignore
        save()

    member this.get(key: 'k) =
        underlyingDict.Item(key)
