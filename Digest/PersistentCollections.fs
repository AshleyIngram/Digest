module Digest.PersistentCollections

open System.Collections.Generic
open System.IO
open Nessos.FsPickler

type PersistentCollection<'t when 't : (new : unit -> 't)>(id, ns) = 
    let pickler = FsPickler.CreateBinary()
    let filepath = "storage/" + id + ns
    
    member this.Load() =
        let filestream = File.OpenRead(filepath)
        pickler.Deserialize<'t>(filestream)

    member this.GetCollection() = if (File.Exists(filepath)) then this.Load() else new 't()
    
    member this.Save(collection) = 
        let fileContents = pickler.Pickle collection
        Directory.CreateDirectory("storage") |> ignore
        let file = File.Create(filepath)
        file.Write(fileContents, 0, fileContents.Length)
        file.Close()

    member this.Delete() =
        File.Delete(filepath)

type PersistentDictionary<'k, 'v when 'k : equality>(id) = 
    inherit PersistentCollection<Dictionary<'k, 'v>>(id, "_dict")

    let collection = base.GetCollection()

    member this.Add(key, value) =
        collection.Add(key, value) |> ignore
        base.Save(collection)
    
    member this.AddOrUpdate(key, value) =
        if (collection.ContainsKey(key)) then
            collection.Remove(key) |> ignore
            collection.Add(key, value)
        else
            collection.Add(key, value) |> ignore
        base.Save(collection)

    member this.Get(key) =
        collection.Item(key)

    member this.GetOrDefault(key) =
        try
            Some(collection.Item(key))
        with
            | :? KeyNotFoundException -> None

    member this.Contains(key) =
        collection.ContainsKey(key)

type PersistentSet<'t>(id) = 
    inherit PersistentCollection<HashSet<'t>>(id, "_set")

    let collection = base.GetCollection()

    member this.Add(value) =
        collection.Add(value) |> ignore
        base.Save(collection)

    member this.Contains(value) =
        collection.Contains(value)

type PersistentQueue<'t>(id) =
    inherit PersistentCollection<Queue<'t>>(id, "_queue")

    let collection = base.GetCollection()

    member this.Enqueue(value) = 
        collection.Enqueue(value)
        base.Save(collection)

    member this.Dequeue() =
        try 
            let value = collection.Dequeue()
            base.Save(collection)
            Some(value)
        with
            | :? System.InvalidOperationException -> None