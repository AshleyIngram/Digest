module Digest.PersistentCollections

open System.Collections.Generic
open System.IO
open Nessos.FsPickler

type PersistentCollection<'t when 't : (new : unit -> 't)>(id, ns) = 
    let pickler = FsPickler.CreateBinary()
    let filepath = "storage/" + id + ns
    
    member this.load() =
        let filestream = File.OpenRead(filepath)
        pickler.Deserialize<'t>(filestream)

    member this.getCollection() = if (File.Exists(filepath)) then this.load() else new 't()
    
    member this.save(collection) = 
        let fileContents = pickler.Pickle collection
        Directory.CreateDirectory("storage") |> ignore
        let file = File.Create(filepath)
        file.Write(fileContents, 0, fileContents.Length)
        file.Close()

type PersistentDictionary<'k, 'v when 'k : equality>(id) = 
    inherit PersistentCollection<Dictionary<'k, 'v>>(id, "_dict")

    let collection = base.getCollection()

    member this.set(key, value) =
        collection.Add(key, value) |> ignore
        base.save(collection)

    member this.get(key) =
        collection.Item(key)

type PersistentSet<'t>(id) = 
    inherit PersistentCollection<HashSet<'t>>(id, "_set")

    let collection = base.getCollection()

    member this.add(value) =
        collection.Add(value) |> ignore
        base.save(collection)

    member this.contains(value) =
        collection.Contains(value)